using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using Big3.Hitbase.MainWindowDesigner.Controls;
using Big3.Hitbase.MainWindowDesigner.Model;

namespace Big3.Hitbase.MainWindowDesigner
{
    public enum UndoAction
    {
        Delete,
        New,
        ChangeProperty
    }

    public class UndoStep
    {
        public string undoName;
        public Stack<UndoEntry> undoEntry = new Stack<UndoEntry>();

        public UndoStep(string undoName)
        {
            this.undoName = undoName;
        }
    }

    public class UndoEntry
    {
        public UndoAction undoAction;
        public int controlID;
        public int parentControlID;
        public int controlIndex;
        public int tabIndex;
        public string property;
        public object value;
        public byte[]  serializedObject;
    }

    public class UndoEngine
    {
        private Stack<UndoStep> undoSteps = new Stack<UndoStep>();

        public Stack<UndoStep> UndoSteps
        {
            get { return undoSteps; }
        }

        private Stack<UndoStep> redoSteps = new Stack<UndoStep>();

        public Stack<UndoStep> RedoSteps
        {
            get { return redoSteps; }
        }

        private UndoStep currentUndoStep = null;
        private MainCDUserControl mainWindowControl;
        private int depth = 0;
        private bool initialized = false;
        private bool undoInAction = false;
        private bool redoInAction = false;

        public UndoEngine(MainCDUserControl dlg)
        {
            mainWindowControl = dlg;
            depth = 0;
        }

        /// <summary>
        /// Muss aufgerufen werden, um die UndoEngine zu starten. Wird normalerweise erst nach dem Ladevorgang
        /// des Dialogs gemacht, weil sonst jeder Vorgang beim Anlegen der internen Dialog-Strukturen 
        /// rückgängig gemacht werden könnte (was ja sinnlos ist).
        /// </summary>
        public void Initialize()
        {
            initialized = true;
        }

        /// <summary>
        /// Fügt einen neuen Eintrag in die Undo-Liste ein, wenn eine Property eines Controls geändert wird.
        /// </summary>
        /// <param name="controlID"></param>
        /// <param name="prop"></param>
        /// <param name="value"></param>
        public void AddPropertyChange(int controlID, string prop, object value)
        {
            if (!initialized)
                return;

            if (currentUndoStep == null)
                throw new Exception("BeginUndoStep must be called first.");

            UndoEntry entry = new UndoEntry();
            entry.undoAction = UndoAction.ChangeProperty;
            entry.controlID = controlID;
            entry.property = prop;
            entry.value = value;
            currentUndoStep.undoEntry.Push(entry);
        }

        /// <summary>
        /// Fügt einen neuen Eintrag in die Undo-Liste ein, wenn ein Control gelöscht wird.
        /// </summary>
        /// <param name="ctl"></param>
        public void AddDelete(HitbaseControl ctl)
        {
            if (!initialized)
                return;

            if (currentUndoStep == null)
                throw new Exception("BeginUndoStep must be called first.");

            UndoEntry entry = new UndoEntry();
            entry.undoAction = UndoAction.Delete;
            entry.controlID = ctl.ControlID;

            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream mem = new MemoryStream();
            bf.Serialize(mem, ctl);
            entry.serializedObject = mem.ToArray();
            mem.Close();

            if (ctl.Parent == null)
                entry.parentControlID = 0;
            else
            {
                entry.parentControlID = ctl.Parent.ControlID;
                entry.controlIndex = ctl.Parent.GetChildIndex(ctl);
                entry.tabIndex = ctl.GetTabIndex();
            }

            currentUndoStep.undoEntry.Push(entry);
        }

        /// <summary>
        /// Fügt einen neuen Eintrag in die Undo-Liste ein, wenn ein neues Control angelegt wird.
        /// </summary>
        /// <param name="ctl"></param>
        public void AddNew(HitbaseControl ctl)
        {
            if (!initialized)
                return;

            if (currentUndoStep == null)
                throw new Exception("BeginUndoStep must be called first.");

            UndoEntry entry = new UndoEntry();
            entry.undoAction = UndoAction.New;
            entry.controlID = ctl.ControlID;
            if (ctl.Parent == null)
                entry.parentControlID = 0;      // Hat keinen Parent
            else
                entry.parentControlID = ctl.Parent.ControlID;
            currentUndoStep.undoEntry.Push(entry);
        }

        /// <summary>
        /// Startet einen neuen Undo-Step. Mehrere Änderungen (UndoEntrys) werden in einem 
        /// Undo-Step zusammengefasst. Muss immer aufgerufen werden, bevor etwas in die Undo-Liste 
        /// eingetragen wird.
        /// </summary>
        /// <param name="undoName"></param>
        public void BeginUndoStep(string undoName)
        {
            if (!initialized)
                return;

            depth++;

            if (currentUndoStep != null)
            {
                return;
            }

            currentUndoStep = new UndoStep(undoName);
        }

        /// <summary>
        /// Beendet den UndoStep und trägt ihn in die Undo-Liste ein.
        /// </summary>
        public void EndUndoStep()
        {
            if (!initialized)
                return;

            depth--;

            if (depth == 0)
            {
                if (currentUndoStep.undoEntry.Count > 0)
                {
                    if (undoInAction)
                        redoSteps.Push(currentUndoStep);
                    else
                    {
                        if (!redoInAction)           // Redo-Buffer löschen
                            redoSteps.Clear();

                        undoSteps.Push(currentUndoStep);
                    }
                }

                currentUndoStep = null;
            }
        }

        /// <summary>
        /// Führt die angegebene Anzahl von Undo-Steps aus.
        /// </summary>
        /// <param name="steps"></param>
        public void DoUndo(int steps)
        {
            if (!initialized)
                return;

            if (undoSteps.Count < 1)           // Nichts da
                return;

            undoInAction = true;

            for (int i = 0; i < steps; i++)
            {
                UndoStep undoStep = undoSteps.Pop();

                BeginUndoStep(undoStep.undoName);
                DoUndoStep(undoStep);
                EndUndoStep();
            }

            //TODO_WPF!!!!!!!!!!!!!!!mainWindowControl.UpdateSelection();

            undoInAction = false;
        }

        /// <summary>
        /// Führt die angegebene Anzahl von Redo-Steps aus.
        /// </summary>
        /// <param name="steps"></param>
        public void DoRedo(int steps)
        {
            if (!initialized)
                return;

            if (redoSteps.Count < 1)           // Nichts da
                return;

            redoInAction = true;

            for (int i = 0; i < steps; i++)
            {
                UndoStep redoStep = redoSteps.Pop();

                BeginUndoStep(redoStep.undoName);
                DoUndoStep(redoStep);
                EndUndoStep();
            }

            //TODO_WPF!!!!!!!!!!!!!!!mainWindowControl.UpdateSelection();

            redoInAction = false;
        }

        /// <summary>
        /// Führt einen einzelnen Undo-Step aus.
        /// </summary>
        /// <param name="step"></param>
        private void DoUndoStep(UndoStep step)
        {
            while (step.undoEntry.Count > 0)
            {
                UndoEntry entry = step.undoEntry.Pop();

                switch (entry.undoAction)
                {
                    case UndoAction.ChangeProperty:
                        {
                            // Property-Änderung rückgängig machen.
                            HitbaseControl hlControl = mainWindowControl.FindHitbaseControlFromID(entry.controlID);

                            mainWindowControl.ChangeProperty(hlControl, entry.property, entry.value);

                            break;
                        }

                    case UndoAction.Delete:
                        {
                            // Löschen eines Objektes rückgängig machen.
                            HitbaseControl newControl = mainWindowControl.AddControlFromStream(entry.parentControlID, entry.controlIndex, entry.serializedObject, true);
                            newControl.SetTabIndex(entry.tabIndex);
                            break;
                        }

                    case UndoAction.New:
                        {
                            HitbaseControl ctl = mainWindowControl.FindHitbaseControlFromID(entry.controlID);
                            HitbaseControl parent;

                            if (entry.parentControlID != 0)
                                parent = mainWindowControl.FindHitbaseControlFromID(entry.controlID);
                            else
                                parent = null;

                            mainWindowControl.DeleteHitbaseControlWithChilds(parent, ctl);
                            break;
                        }
                }
            }
        }
    }
}

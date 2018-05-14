using System;
using System.Collections.Generic;
using System.Text;
using Big3.Hitbase.SharedResources;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Big3.Hitbase.DataBaseEngine
{
    public enum Operator
    {
        None,
        Greater,
        GreaterEqual,
        Contains,
        Equal,
        LessEqual,
        Less,
        NotEqual,
        NotContains,
        StartsWith,
        Empty,
        NotEmpty
    }

    public enum Logical
    {
        None,
        And,
        Or
    }

    /// <summary>
    /// Definiert eine einzelne Suchbedingung für z.B. einen Filter
    /// </summary>
    [Serializable]
    public class SingleCondition : INotifyPropertyChanged
    {
        public SingleCondition()
        {

        }

        public SingleCondition(Field field, Operator op, object value)
        {
            Field = field;
            Operator = op;
            Value = value;
            Logical = Logical.None;
        }

        public SingleCondition(Field field, Operator op, object value, Logical logical)
        {
            Field = field;
            Operator = op;
            Value = value;
            Logical = logical;
        }

        private Field field;
        public Field Field
        {
            get { return field; }
            set
            {
                field = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Field"));
                    PropertyChanged(this, new PropertyChangedEventArgs("AvailableOperators"));
                }
            }
        }

        private Operator _operator;
        public Operator Operator
        {
            get { return _operator; }
            set
            {
                _operator = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Operator"));
            }
        }

        private object _value;
        public object Value
        {
            get { return _value; }
            set
            {
                _value = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Value"));
            }
        }

        private Logical logical;
        public Logical Logical
        {
            get { return logical; }
            set
            {
                logical = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Logical"));
            }
        }

        /// <summary>
        /// Liefert alle Operatoren zurück, die zum angegebenen Feld passen.
        /// </summary>
        /// <returns></returns>
        public OperatorModel[] AvailableOperators
        {
            get
            {
                List<OperatorModel> ops = new List<OperatorModel>();

                foreach (Operator op in Enum.GetValues(typeof(Operator)))
                {
                    if (op == Operator.None)
                        continue;

                    // Bei ntext Datenbank-Feldern nur "Contains"-Varianten erlauben
                    if ((field == Field.Comment || field == Field.TrackComment || field == Field.TrackLyrics) && 
                        op != Operator.Contains && op != Operator.NotContains && op != Operator.StartsWith)
                        continue;

                    ops.Add(new OperatorModel() { Operator = op, DisplayName = Big3.Hitbase.DataBaseEngine.Condition.GetNameOfOperator(op) });
                }

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Operator"));

                return ops.ToArray();
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class OperatorModel : INotifyPropertyChanged
    {
        private Operator _operator;
        public Operator Operator
        {
            get { return _operator; }
            set
            {
                _operator = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Operator"));
            }
        }


        private string displayName;
        public string DisplayName
        {
            get { return displayName; }
            set
            {
                displayName = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("DisplayName"));
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
    }

    /// <summary>
    /// Definiert die Suchbedingung für z.B. einen Filter
    /// </summary>
    [Serializable]
    public class Condition : ObservableCollection<SingleCondition>
    {
        public void Add(Field field, Operator op, object value)
        {
            Add(new SingleCondition(field, op, value));
        }

        /// <summary>
        /// Liefert die aktuelle Bedingung als Text zurück.
        /// </summary>
        /// <returns></returns>
        public string GetConditionString(DataBase db)
        {
            string conditionString = "";
            
            foreach (SingleCondition cond in this)
            {
                if (!string.IsNullOrEmpty(conditionString))
                {
                    if (cond.Logical == Logical.Or)
                        conditionString += " " + StringTable.Or + " ";
                    else
                        conditionString += " " + StringTable.And + " ";
                }

                string fieldName = db.GetNameOfField(cond.Field);
                string opName = GetNameOfOperator(cond.Operator);

                string singleCondString = fieldName + " " + opName + " ";

                switch (cond.Field)
                {
                    case Field.Date:
                        singleCondString += "'" + Big3.Hitbase.Miscellaneous.Misc.FormatDate((string)cond.Value) + "'";
                        break;
                    case Field.TotalLength:
                    case Field.TrackLength:
                        singleCondString += Big3.Hitbase.Miscellaneous.Misc.GetLongTimeString((int)cond.Value);
                        break;
                    case Field.Price:
                        singleCondString += Big3.Hitbase.Miscellaneous.Misc.FormatCurrencyValue((int)cond.Value);
                        break;
                    default:
                        {
                            if (cond.Value is string)
                            {
                                singleCondString += "'" + cond.Value.ToString() + "'";
                            }
                            else
                            {
                                singleCondString += cond.Value.ToString();
                            }

                            break;
                        }
                }

                conditionString += singleCondString;
            }

            if (string.IsNullOrEmpty(conditionString))
                return "<" + StringTable.None + ">";

            return conditionString;
        }

        /// <summary>
        /// Sucht in der Condition nach dem angegebenen Feld und dem Operator.
        /// </summary>
        /// <param name="field"></param>
        /// <returns>Die SingleCondition oder null, wenn nichts gefunden wurde.</returns>
        public SingleCondition FindByField(Field field, Operator op)
        {
            foreach (SingleCondition s in this)
            {
                if (s.Field == field && s.Operator == op)
                    return s;
            }

            return null;
        }

        /// <summary>
        /// Sucht in der Condition nach dem angegebenen Feld.
        /// </summary>
        /// <param name="field"></param>
        /// <returns>Die SingleCondition oder null, wenn nichts gefunden wurde.</returns>
        public SingleCondition FindByField(Field field)
        {
            foreach (SingleCondition s in this)
            {
                if (s.Field == field)
                    return s;
            }

            return null;
        }

        /// <summary>
        /// Liefert alle vorhandenen Operatoren zurück.
        /// </summary>
        /// <returns></returns>
        public static Operator[] GetAllOperators()
        {
            List<Operator> ops = new List<Operator>();

            foreach (Operator op in Enum.GetValues(typeof(Operator)))
            {
                ops.Add(op);
            }

            return ops.ToArray();
        }

        /// <summary>
        /// Liefert die String-Repräsentation des angegebenen Operators zurück.
        /// </summary>
        /// <param name="Operator"></param>
        /// <returns></returns>
        public static string GetNameOfOperator(Operator Operator)
        {
            switch (Operator)
            {
                case Operator.Greater:
                    return ">";
                case Operator.GreaterEqual:
                    return ">=";
                case Operator.Contains:
                    return StringTable.Contains;
                case Operator.Equal:
                    return "=";
                case Operator.LessEqual:
                    return "<=";
                case Operator.Less:
                    return "<";
                case Operator.NotEqual:
                    return "<>";
                case Operator.NotContains:
                    return StringTable.NotContains;
                case Operator.StartsWith:
                    return StringTable.StartsWith;
                case Operator.Empty:
                    return StringTable.OperatorEmpty;
                case Operator.NotEmpty:
                    return StringTable.OperatorNotEmpty;
                default:
                    return "<UNKNOWN OPERATOR>";
            }
        }

        /// <summary>
        /// Liefert die String-Repräsentation für Datenbankabfragen des angegebenen Operators zurück.
        /// </summary>
        /// <param name="Operator"></param>
        /// <returns></returns>
        public static string GetNameOfOperatorForFilter(Operator Operator)
        {
            switch (Operator)
            {
                case Operator.Greater:
                    return ">";
                case Operator.GreaterEqual:
                    return ">=";
                case Operator.Contains:
                    return "LIKE";
                case Operator.Equal:
                    return "=";
                case Operator.LessEqual:
                    return "<=";
                case Operator.Less:
                    return "<";
                case Operator.NotEqual:
                    return "<>";
                case Operator.NotContains:
                    return "NOT LIKE";
                case Operator.StartsWith:
                    return "LIKE";
                case Operator.Empty:
                    return "Is NULL";
                case Operator.NotEmpty:
                    return "Is NOT NULL";
                default:
                    return "<UNKNOWN OPERATOR>";
            }
        }

        /// <summary>
        /// Prüft, ob die Condition gültig ist.
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            foreach (SingleCondition cond in this)
            {
                if (cond.Field == Field.None)
                    return false;
                if (cond.Operator == Operator.None)
                    return false;

                // TODO: Hier noch prüfen, ob der Value vom richtigen Typ ist.
            }

            return true;
        }

        /// <summary>
        /// Liest die Condition aus der Registry
        /// </summary>
        /// <param name="regKey"></param>
        public void RestoreFromRegistry(string regKey)
        {
            //!!!!!!!!!!!!!!!!!!!
        }

        /// <summary>
        /// Speichert die Condition in die Registry
        /// </summary>
        /// <param name="regKey"></param>
        public void SaveToRegistry(string regKey)
        {
            //!!!!!!!!!!!!!!!!!!!
        }

        /// <summary>
        /// Verbindet (addiert) beide Conditions
        /// </summary>
        /// <param name="cond1"></param>
        /// <param name="cond2"></param>
        /// <returns></returns>
        public static Condition Combine(Condition cond1, Condition cond2)
        {
            Condition combinedCondition = new Condition();

            foreach (SingleCondition singleCond in cond1)
            {
                if (singleCond.Field != Field.None)
                {
                    // Der letzte braucht eine "AND"-Verknüpfung, damit beide Conditions mit AND verknüpft sind.
                    if (singleCond == cond1[cond1.Count - 1])
                        combinedCondition.Add(new SingleCondition(singleCond.Field, singleCond.Operator, singleCond.Value, Logical.And));
                    else
                        combinedCondition.Add(new SingleCondition(singleCond.Field, singleCond.Operator, singleCond.Value, singleCond.Logical));
                }
            }

            if (cond2 != null)
            {
                foreach (SingleCondition singleCond in cond2)
                {
                    combinedCondition.Add(new SingleCondition(singleCond.Field, singleCond.Operator, singleCond.Value, singleCond.Logical));
                }
            }

            return combinedCondition;
        }
    }
}

   M S B A M L   `   `   `  ���� M  ILibraryManagerDemo, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullo<clr-namespace:Windows7.DesktopIntegration.LibraryManagerDemo.Windows7.DesktopIntegration.LibraryManagerDemo  ?    9Windows7.DesktopIntegration.LibraryManagerDemo.MainWindow   \ XPresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    8x,http://schemas.microsoft.com/winfx/2006/xaml    Llocal<clr-namespace:Windows7.DesktopIntegration.LibraryManagerDemo      � Title$  Library Manager��$	��600��$	��800�� � 	Resources B   <Windows7.DesktopIntegration.LibraryManagerDemo.TextConverter   TextConverterH   BWindows7.DesktopIntegration.LibraryManagerDemo.NullToBoolConverter  NullToBoolConverter��%   &	        &	          .���  � RowDefinitions �� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ���  � ColumnDefinitions �� $��225.432* ��� $��219.978* �.+��� -   $��_openLibraryButton��$��23��$��12q�$��0��$��0��$	��Top=�.��Open Library�� -   $��_createLibraryButton��$��23��$��12q�$��0��$��1��$��Bottom=�.��Create New Library�� -   $"��_addDirectoryToLibraryButton��$��23��$��12q�$��1��$��0��$	��Top=�{���  �� ElementName$ _textBlockName�� �� Path$
 Text� �� 	Converter# [  .��Add Directory�� -   $'��!_removeDirectoryFromLibraryButton��$��23��$��12q�$��1��$��1��$��Bottom=�{��� $ _foldersListBox��$ SelectedItem�# [ .��Remove Directory�� -   $ ��_openLibraryManageUIButton��$��23��$��12q�$��0��$��2��$��Bottom=�{��� $ _textBlockName��$
 Text�# [  .��Open Library Manage UI�� -   $!��_setDefaultSaveFolderButton��$��23��$��12q�$��1��$��2��$��Bottom=�{��� $ _foldersListBox��$ SelectedItem�# [ .��Set Default Save Folder�� -   $��_deleteLibraryButton��$��23��$��12q�$��0��$��3��$��Bottom=�{��� $ _textBlockName��$
 Text�# [  .��Delete Library�� -   $��_setFolderTypeComboBox��$��23��$��12q�$��1��$��3��$��Bottom=�{��� $ _textBlockName��$
 Text�# [  ���� $ FolderTypeList� �� SelectedValue �� $ 
FolderType��� -	   $%��_pinnedToNavigationPaneCheckBox��$��23��$��12q�$��0��$��4��$��Bottom=�{��� $ _textBlockName��$
 Text�# [   d� 	IsChecked �� $ IsPinnedToNavigationPane�.��Is Pinned To Navigation Pane�� -
   $��_setLibraryIcon��$��23��$��12q�$��1��$��4��$��Bottom=�{��� $ _textBlockName��$
 Text�# [  .���� -   $��
_imageIcon������ $
 Icon��� 	 �� TextAlignment$	 Center=�$��0��$��5��$
��None{�
 �� Padding$
 12q�$��39.996��$	��Top=�Name:�� $
 12q�$��0��$��6��$	 Center=�$
��None{�$��39.996��$	��Top=�Save Directory:�� -   $��_textBlockName��$
 12q�$��1��$��5��$
	 Left=�$
��None{�$��39.996��$	��Top=����� $ LibraryName��� -   $$��_textBlockDefaultSaveDirectory��$
 12q�$��1��$��6��$
	 Left=�$
��None{�$��39.996��$	��Top=����� $ DefaultSaveFolder��� $
 12q�$��7��$	 Center=�$
��None{�$��2��
Folders:�� -   $��_foldersListBox��$��8��$��2������ $ 
FolderList�
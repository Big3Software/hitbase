   M S B A M L   `   `   `  ����M  ILibraryManagerDemo, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullo<clr-namespace:Windows7.DesktopIntegration.LibraryManagerDemo.Windows7.DesktopIntegration.LibraryManagerDemo  5      ?    9Windows7.DesktopIntegration.LibraryManagerDemo.MainWindow   5   	   \ XPresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      8x,http://schemas.microsoft.com/winfx/2006/xaml    5      Llocal<clr-namespace:Windows7.DesktopIntegration.LibraryManagerDemo    5        � Title$  Library Manager��5      $	��600��6   $	��800��6*    � 	Resources 5      B   <Windows7.DesktopIntegration.LibraryManagerDemo.TextConverter   TextConverterH   BWindows7.DesktopIntegration.LibraryManagerDemo.NullToBoolConverter  NullToBoolConverter��%:   &	        &	        5   
   6     5   
   6$   5	      .��5      �  � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5       � ColumnDefinitions 5   
   �� 5      $��225.432* �6   �� 5      $��219.978* �6   .+�5   
   �� -   5        $��_openLibraryButton��5   G   $��23��6   $��12q�6   $��0��6)   $��0��69   $	��Top=�6a   .��6�   Open Library6�   �� 5   
   -   5        $��_createLibraryButton��5   F   $��23��6   $��12q�6   $��0��6)   $��1��69   $��Bottom=�6�   .��6�   Create New Library6�   �� 5   
   -   5        $"��_addDirectoryToLibraryButton��5   F   $��23��6   $��12q�6   $��1��6)   $��0��69   $	��Top=�6�   {�6j   ��  �� ElementName$ _textBlockName�� �� Path$
 Text� �� 	Converter# [  .��6  Add Directory6  �� 5   
   -   5        $'��!_removeDirectoryFromLibraryButton��5   F   $��23��6   $��12q�6   $��1��6)   $��1��69   $��Bottom=�6  {�6o   �� $ _foldersListBox��$ SelectedItem�# [ .��6'  Remove Directory69  �� 5   
   -   5        $ ��_openLibraryManageUIButton��5   F   $��23��6   $��12q�6   $��0��6)   $��2��69   $��Bottom=�6�   {�6h   �� $ _textBlockName��$
 Text�# [  .��6
  Open Library Manage UI6"  �� 5    
   -   5        $!��_setDefaultSaveFolderButton��5    F   $��23��6   $��12q�6   $��1��6)   $��2��69   $��Bottom=�6   {�6i   �� $ _foldersListBox��$ SelectedItem�# [ .��6  Set Default Save Folder64  �� 5!   
   -   5        $��_deleteLibraryButton��5!   F   $��23��6   $��12q�6   $��0��6)   $��3��69   $��Bottom=�6�   {�6b   �� $ _textBlockName��$
 Text�# [  .��6  Delete Library6  �� 5"   
   -   5        $��_setFolderTypeComboBox��5"   H   $��23��6   $��12q�6   $��1��6+   $��3��6;   $��Bottom=�6#  {�6f   �� $ _textBlockName��$
 Text�# [  ��6�   �� $ FolderTypeList� �� SelectedValue 6�   �� $ 
FolderType�6@  �� 5#   
   -	   5        $%��_pinnedToNavigationPaneCheckBox��5#   H   $��23��6   $��12q�6   $��0��6+   $��4��6;   $��Bottom=�6
  {�6o   �� $ _textBlockName��$
 Text�# [   d� 	IsChecked 6�   �� $ IsPinnedToNavigationPane�.��6%  Is Pinned To Navigation Pane6C  �� 5$   
   -
   5        $��_setLibraryIcon��5$   F   $��23��6   $��12q�6   $��1��6)   $��4��69   $��Bottom=�6�   {�6]   �� $ _textBlockName��$
 Text�# [  .��5%      �� -   5        $��
_imageIcon��5%      ��6'   �� $
 Icon�5&      �� 5'   
   	 �� TextAlignment$	 Center=�6   $��0��6+   $��5��6;   $
��None{�6H   
 �� Padding$
 12q�6_   $��39.996��6l   $	��Top=�6|   Name:6�   �� 5(   
   $
 12q�6   $��0��6!   $��6��61   $	 Center=�6>   $
��None{�6U   $��39.996��6l   $	��Top=�6|   Save Directory:6�   �� 5)   
   -   5        $��_textBlockName��5)      $
 12q�6*   $��1��67   $��5��6G   $
	 Left=�6T   $
��None{�6i   $��39.996��6�   $	��Top=�6�   ��6�   �� $ LibraryName�6�   �� 5*   
   -   5        $$��_textBlockDefaultSaveDirectory��5*      $
 12q�6:   $��1��6G   $��6��6W   $
	 Left=�6d   $
��None{�6y   $��39.996��6�   $	��Top=�6�   ��6�   �� $ DefaultSaveFolder�6�   �� 5+   
   $
 12q�6   $��7��6!   $	 Center=�6.   $
��None{�6E   $��2��6\   
Folders:6z   �� 5,   
   -   5        $��_foldersListBox��5,      $��8��6   $��2��6^   ��66   �� $ 
FolderList�6^   5-      5/      
   M S B A M L   `   `   `  ����O  KFussballDeVisualizer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nulle7clr-namespace:FussballDeVisualizer.Views.MainWindowTabs)FussballDeVisualizer.Views.MainWindowTabs  5      k:clr-namespace:FussballDeVisualizer.ViewModels.PopupHelpers,FussballDeVisualizer.ViewModels.PopupHelpers  5      <    6FussballDeVisualizer.Views.MainWindowTabs.TableTabView   5      W SPresentationCore, Version=7.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=7.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=7.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=7.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=7.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Ilocal7clr-namespace:FussballDeVisualizer.Views.MainWindowTabs     5      Spopuphelpers:clr-namespace:FussballDeVisualizer.ViewModels.PopupHelpers     5        � 	Resources  5
         PositiveTemplate  NegativeTemplateI   CFussballDeVisualizer.ViewModels.PopupHelpers.PunkteTemplateSelector  PunkteTemplateSelector   PositiveTemplate   NegativeTemplate��%  &	        &	 b     &	 �     7   7  �� 5   
   .R�5      �� 
��� � �$��Center=�6B   ��6   �� *Value+6B   5      �� 5   
   .R�5      �� 
���  ��$��Center=�6@   ��6   �� *Value+6@   5        5   
   8   8  6�   5      .��5      �  � RowDefinitions 5   
   �� 5      $��2* �6   �� 5      $��3* �6   .+�5!   
   &   System.Windows.Controls.DataGrid  $��0��6      AutoGenerateColumns$ False��5"         SelectionMode$ Extended=�5#         CanUserAddRows$ False��5$         CanUserDeleteRows$ False��5%         
IsReadOnly$
 True��5&      ��5!       �� *Liga.Tabelle+	   Columns	 5'      0  *System.Windows.Controls.DataGridTextColumn  5(      ,  &System.Windows.Controls.DataGridColumn
   Header$

 Rang��6%   1  +System.Windows.Controls.DataGridBoundColumn   Binding 63   �� *Rang+  5)      $
 
Mannschaft��6%    69   �� *
Mannschaft+  5*      $
 Spiele��6%    65   �� *Spiele+  5+      $
 Gewonnen��6%    67   �� *
Gewonnen+  5,      $
 Unentschieden��6%    6<   �� *Unentschieden+  5-      $
 Verloren��6%    67   �� *
Verloren+  5.      $

 Tore��6%    63   �� *Tore+  5/      $
 	Gegentore��6%    68   �� *	Gegentore+  50      $
 Tordifferenz��6%    6;   �� *Tordifferenz+  51      $
 Punkte��6%    65   �� *Punkte+  52      $
 	Vorsprung��6%    68   �� *	Vorsprung+  53      $
 
Rueckstand��6%    69   �� *
Rueckstand+55      �� 57   
   $��1��6   ��6&   �� *Matrix+58      59      
   M S B A M L   `   `   `  ����G  CCodeFox_Shop, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:CodeFox_ShopCodeFox_Shop  5   	   #    CodeFox_Shop.ConnectSQLWindow   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   ,localclr-namespace:CodeFox_Shop     5   	     � Title$#  Csatlakozás az adatbázishoz��5   	   $	��350��6-   $	��400��6=   $	��350��6J   $	��400��6Y   .��5	      �  � RowDefinitions 5
   
   �� 5      $��* �6   �� 5      $��* �6   �� 5      $��* �6   �� 5      $��* �6   �� 5      $��* �6   �� 5      $��* �6   �� 5      $��* �6   �� 5      $��* �6   �� 5      $��* �6   .+�5   
   �� $��Szerver IP címe��6   $��0��6*   $��Center=�67   $��Center=�6T   �� 5   
   -   5         ��Name$ ServerInput��5      $��1��6'   �� 5   
   $��Adatbázis neve��6   $��2��6)   $��Center=�66   $��Center=�6S   �� 5   
   -   5        $ DatabaseInput��5      $��3��6)   �� 5   
   $��Felhasználónév��6   $��4��6)   $��Center=�66   $��Center=�6S   �� 5   
   -   5        $ UsernameInput��5      $��5��6)   �� 5   
   $��Jelszó��6   $��6��6!   $��Center=�6.   $��Center=�6K   G� 5   
   -   5         G�Name$ PasswordInput��5      $��7��6-   �� 5   
   -   5         ��Name$ ConnectDatabaseButton��5      $��Csatlakozás��60   
�������$��8��6]   6j   5       5!      
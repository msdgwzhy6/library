<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="LibrarySeatsManager.LoginPage" Theme="MyTheme" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>图书馆座位管理系统登录</title>
    <style type="text/css">
        .style1
        {
            width: 74px;
        }
        .style2
        {
            width: 203px;
        }
    </style>
</head>
<body>
<form id="Form2" runat="server">
    <div>
          <asp:Label ID="Label3" runat="server" Font-Size="XX-Large" Text="图书馆座位管理系统"></asp:Label>
          <br />
          <br />
          <asp:Label ID="Label4" runat="server" Font-Size="X-Large" Text="用户登录"></asp:Label>
          <br />
          <br />   
        <table style="width:280px" align="center">
            <tr align="center">
                <td class="style1">
                    <asp:Label ID="Label1" runat="server" Text="用户名"></asp:Label></td>
                <td class="style2">
                    <asp:TextBox ID="UsernameText" runat="server" 
                        Width="150px" Height="30px" style="margin-left: 0px; margin-right: 0px;"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="style1" align="center">
                  <asp:Label ID="Label2" runat="server" Text="密码"></asp:Label></td>
                <td class="style2" align="center">
                    <asp:TextBox ID="PasswordText" runat="server" Width="150px" TextMode="Password" 
                        Height="30px" style="margin-left: 0px" ></asp:TextBox>
                </td>
            </tr>
            <tr align="center">
                <td class="style1">
                    </td>
                <td class="style2">
                    <asp:Button ID="Login" runat="server" onclick="Login_Click" Text="登录" 
                        Width="59px" Height="30px" Font-Size="Medium" />
                    <asp:Button ID="Register" runat="server" onclick="Register_Click" Text="注册" 
                        Width="60px" Height="30px" Font-Size="Medium" />
                </td>
            </tr>
        </table>
   
   </div>
    </form>
  </body>
</html>

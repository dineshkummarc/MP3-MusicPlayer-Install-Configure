<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Please log in</title>
</head>
<body>
    <form id="form1" runat="server">
    <div align="center">
            <asp:ChangePassword ID="changePassword" runat="server" BackColor="#F7F7DE" BorderColor="#CCCC99"
                BorderStyle="Solid" BorderWidth="1px" ChangePasswordTitleText='Please change the password'
                Font-Names="Verdana" Font-Size="10pt" PasswordLabelText="Old Password:" InstructionText="Default password is &quot;<b>1234567</b>&quot;, and <br>since it's the same for every installation of the program,<br>it's not secure. Please set your own password.<hr>" OnChangingPassword="changePassword_ChangingPassword">
                <TitleTextStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
            </asp:ChangePassword>
        <asp:Login ID="Login1" runat="server" BackColor="#F7F7DE" BorderColor="#CCCC99" BorderStyle="Solid"
            BorderWidth="1px" Font-Names="Verdana" Font-Size="10pt" OnAuthenticate="Login1_Authenticate" RememberMeSet="True">
            <TitleTextStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
        </asp:Login>
    </div>
    </form>
</body>
</html>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="PracticalAssignmentApplicationSecurity_202920H.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="https://www.google.com/recaptcha/api.js?render=6LdxTVkeAAAAAG1eSvY9cBBivoNSxP2c_hgt0YkM"></script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
             <h2>Login Form</h2>
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" ForeColor="#cc0000"/>
       <br />
        <table class="style1">
            <tr>
                <td class="style3">
                    <asp:Label ID="lb_email" runat="server" Text="Email Address"></asp:Label>
                </td>
                <td class="style2">
                    <asp:TextBox ID="tb_email" runat="server" Width="240px" Height="30px" placeholder="Enter your email address"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="This field is required" ControlToValidate="tb_email" ForeColor="#cc0000"></asp:RequiredFieldValidator>
                </td>
                <asp:Label ID="fb_email" runat="server"></asp:Label>
            </tr>
            <tr>
                <td class="style3">
                    <asp:Label ID="lb_password" runat="server" Text="Password"></asp:Label>
                </td>
                <td class="style2">
                    <asp:TextBox ID="tb_password" runat="server" Width="240px" Height="30px" placeholder="Enter your password" TextMode="Password"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="This field is required" ControlToValidate="tb_password" ForeColor="#cc0000"></asp:RequiredFieldValidator>
                </td>
                <asp:Label ID="fb_password" runat="server"></asp:Label>
            </tr>
            <tr>
                <td class="style4">
                    
                </td>
                <td class="style5">
                      <input type="hidden" id="g-recaptcha-response" name="g-recaptcha-response" />

                </td>
            </tr>
            <tr>
                <td class="style4">
                    
                </td>
                <td class="style5">
                      <asp:Button runat="server" ID="submit_btn" Text="Login" OnClick="submit_btnClick"  />

                </td>
            </tr>
                         

            </table>
            
             
            <br />
             <asp:Label ID="RegisterLink" runat="server">New to this website? <a href="Registration.aspx" style="color:red;"><b>Sign Up</b></a></asp:Label>
            
            <br />
            
            <br />
            <asp:Label ID="error_message" runat="server"></asp:Label>
            <br />
            <asp:Label ID="error_captcha" runat="server"></asp:Label>
            <br />
            <asp:Label ID="lb_gScore" runat="server" EnableViewState="False"></asp:Label>
        </div>
    </form>

    <script>
        grecaptcha.ready(function () {
            grecaptcha.execute('6LdxTVkeAAAAAG1eSvY9cBBivoNSxP2c_hgt0YkM', { action: 'Login' }).then(function (token) {
                document.getElementById("g-recaptcha-response").value = token;
            });
        });
    </script>
</body>
</html>

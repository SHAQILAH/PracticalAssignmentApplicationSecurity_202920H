<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Registration.aspx.cs" Inherits="PracticalAssignmentApplicationSecurity_202920H.Registration" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">
        function validate() {
            var str = document.getElementById('<%=tb_password.ClientID %>').value;
            if (str.length < 12) {
                document.getElementById("fb_password").innerHTML = "Password Length Must Be At Least 12 Characters";
                document.getElementById("fb_password").style.color = "Red";
                return ("too_short");
            }
            else if (str.search(/[0-9]/) == -1) {
                document.getElementById("fb_password").innerHTML = "Password require at least 1 number";
                document.getElementById("fb_password").style.color = "Red";
                return ("no_number");
            }
            else if (str.search(/[A-Z]/) == -1) {
                document.getElementById("fb_password").innerHTML = "Password require at least 1 uppercase";
                document.getElementById("fb_password").style.color = "Red";
                return ("no_uppercase");
            }
            else if (str.search(/[a-z]/) == -1) {
                document.getElementById("fb_password").innerHTML = "Password require at least 1 lowercase";
                document.getElementById("fb_password").style.color = "Red";
                return ("no_lowercase");
            }
            else if (str.search(/[^a-zA-Z0-9]/) == -1) {
                document.getElementById("fb_password").innerHTML = "Password require at least 1 special characters";
                document.getElementById("fb_password").style.color = "Red";
                return ("no_specialcharacters");
            }
            document.getElementById("fb_password").innerHTML = "Excellent!";
            document.getElementById("fb_password").style.color = "Blue";
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h2>Registration Form</h2>
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" ForeColor="#cc0000"/>
       <br />
        <table class="style1">
            <tr>
                <td class="style3">
                    <asp:Label ID="lb_fname" runat="server" Text="First Name"></asp:Label>
                </td>
                <td class="style2">
                    <asp:TextBox ID="tb_fname" runat="server" Width="240px" Height="30px" placeholder="Enter your first name"></asp:TextBox>
<%--                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="This field is required" ControlToValidate="tb_fname" ForeColor="#cc0000"></asp:RequiredFieldValidator>--%>
                <asp:Label ID="fb_fname" runat="server"></asp:Label>
                </td>
                
            </tr>
            <tr>
                <td class="style3">
                    <asp:Label ID="lb_lname" runat="server" Text="Last Name"></asp:Label>
                </td>
                <td class="style2">
                    <asp:TextBox ID="tb_lname" runat="server" Width="240px" Height="30px" placeholder="Enter your last name"></asp:TextBox>
<%--                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="This field is required" ControlToValidate="tb_lname" ForeColor="#cc0000"></asp:RequiredFieldValidator>--%>
                    <asp:Label ID="fb_lname" runat="server"></asp:Label>
                </td>
                
            </tr>
            <tr>
                <td class="style3">
                    <asp:Label ID="lb_email" runat="server" Text="Email Address"></asp:Label>
                </td>
                <td class="style2">
                     <asp:TextBox ID="tb_email" runat="server" Width="240px" Height="30px" placeholder="Enter your email address" TextMode="Email"></asp:TextBox>
                    <asp:Label ID="fb_emailGeneral" runat="server"></asp:Label>
                    <asp:Label ID="fb_email" runat="server"></asp:Label>
<%--                     <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="This field is required" ControlToValidate="tb_email" ForeColor="#cc0000"></asp:RequiredFieldValidator>--%>
<%--                     <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ErrorMessage="Invalid format of email entered!" ControlToValidate="tb_email" ValidationExpression="^\w+[\+\.\w-]*@([\w-]+\.)*\w+[\w-]*\.([a-z]{2,4}|\d+)$"></asp:RegularExpressionValidator>--%>
                </td>
                
            </tr>
            <tr>
                <td class="style3">
                    <asp:Label ID="lb_credit" runat="server" Text="Credit Card"></asp:Label>
                </td>
                <td class="style2">
                    <asp:TextBox ID="tb_credit" runat="server" Width="240px" Height="30px" placeholder="Enter your credit card information"></asp:TextBox>
<%--                    <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ErrorMessage="This field is required" ControlToValidate="tb_credit" ForeColor="#cc0000"></asp:RequiredFieldValidator>--%>
<%--                    <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ErrorMessage="Invalid credit card format used" ControlToValidate="tb_credit" ValidationExpression="^(?:4[0-9]{12}(?:[0-9]{3})?|5[1-5][0-9]{14}|6(?:011|5[0-9][0-9])[0-9]{12}|3[47][0-9]{13}|3(?:0[0-5]|[68][0-9])[0-9]{11}|(?:2131|1800|35\d{3})\d{11})$"></asp:RegularExpressionValidator>--%>
                    <asp:Label ID="fb_credit" runat="server"></asp:Label>
                </td>
                
            </tr>
             <tr>
                <td class="style3">
                    <asp:Label ID="lb_password" runat="server" Text="Password"></asp:Label>
                </td>
                <td class="style2">
                    <!-- For client-based checks -->
                    <%--<asp:TextBox ID="tb_password" runat="server" Width="240px" Height="30px" placeholder="Enter your password" TextMode="Password" onkeyup="javascript:validate()"></asp:TextBox>--%>
                    <!-- For server-based checks -->
                     <asp:TextBox ID="tb_password" runat="server" Width="240px" Height="30px" placeholder="Enter your password" TextMode="Password"></asp:TextBox> 
<%--                    <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ErrorMessage="This field is required" ControlToValidate="tb_password" ForeColor="#cc0000"></asp:RequiredFieldValidator>--%>
                     <asp:Label ID="fb_passwordGeneral" runat="server"></asp:Label>
                </td>
                 <asp:Label ID="fb_password" runat="server"></asp:Label>
            </tr>
            
             <tr>
                <td class="style3">
                    <asp:Label ID="lb_dob" runat="server" Text="Date of Birth"></asp:Label>
                </td>
                <td class="style2">
                    <asp:TextBox ID="tb_birthdate" runat="server" Width="240px" Height="30px" placeholder="Enter your birth date" TextMode="Date"></asp:TextBox>
<%--                    <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ErrorMessage="This field is required" ControlToValidate="tb_birthdate" ForeColor="#cc0000"></asp:RequiredFieldValidator>--%>
                    
                    <asp:Label ID="fb_dob" runat="server"></asp:Label>
                </td>
                 
            </tr>
            <tr>
                <td class="style3">
                    <asp:Label ID="lb_photo" runat="server" Text="Upload Photo"></asp:Label>
                </td>
                <td class="style2">
                     <%--<asp:FileUpload ID="fu_photo" runat="server" accept=".png,.jpg,.jpeg,.gif" />--%>
                     <asp:FileUpload ID="fu_photo" runat="server" />
                    
                        <%--<asp:RegularExpressionValidator ID="RegExValFileUploadFileType" runat="server"
                        ControlToValidate="fu_photo"
                        ErrorMessage="Only .jpg,.png,.jpeg,.gif Files are allowed" Font-Bold="True"
                        Font-Size="Medium"
                        ValidationExpression="(.*?)\.(jpg|jpeg|png|gif|JPG|JPEG|PNG|GIF)$"></asp:RegularExpressionValidator>--%>
                <asp:Label ID="fb_photo" runat="server"></asp:Label>
                </td>
                
            </tr>
             <tr>
                <td class="style4">
                    
                </td>
                <td class="style5">
                      <asp:Button runat="server" ID="submit_btn" Text="Register" OnClick="submit_btnClick" />

                </td>
            </tr>

        </table>
        </div>
        <%--<p>
            <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ErrorMessage="Password Complexity - Weak" ControlToValidate="tb_password" ValidationExpression="^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[$@$!%*?&])[A-Za-z\d$@$!%*?&]{12,}"></asp:RegularExpressionValidator>
        </p>
        <p>
            <asp:Label ID="lb_error" runat="server"></asp:Label>
        </p>--%>
    </form>
</body>
</html>

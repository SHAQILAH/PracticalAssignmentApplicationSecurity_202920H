<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ChangePassword.aspx.cs" Inherits="PracticalAssignmentApplicationSecurity_202920H.ChangePassword" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    
    <div>
        <h2><asp:Label ID="lb_change" runat="server">Change Password</asp:Label></h2>
             
            <%--<asp:ValidationSummary ID="ValidationSummary1" runat="server" ForeColor="#cc0000"/>--%>
       <br />
        <table class="style1">
            <tr>
                <td class="style3" style="width: 130px; height: 87px;">
                    <asp:Label ID="lb_currentpassword" runat="server" Text="Current Password"></asp:Label>
                </td>
                <td class="style2" style="height: 87px">
                    <asp:TextBox ID="tb_currentpassword" runat="server" Width="240px" Height="44px" placeholder="Enter your current password" TextMode="Password"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="This field is required" ControlToValidate="tb_currentpassword" ForeColor="#cc0000"></asp:RequiredFieldValidator>
                    <asp:Label ID="fb_currentpassword" runat="server"></asp:Label>
                </td>
                
            </tr>
            <tr>
                <td class="style3" style="width: 130px">
                    <asp:Label ID="lb_newpassword" runat="server" Text="New Password"></asp:Label>
                </td>
                <td class="style2">
                    <asp:TextBox ID="tb_newpassword" runat="server" Width="240px" Height="44px" placeholder="Enter your new password" TextMode="Password"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="This field is required" ControlToValidate="tb_newpassword" ForeColor="#cc0000"></asp:RequiredFieldValidator>

                    <asp:Label ID="fb_newpassword" runat="server"></asp:Label>

                </td>
                <asp:Label ID="fb_error" runat="server"></asp:Label>
            </tr>
            
            <tr>
                <td class="style4" style="width: 130px">
                    
                </td>
                <td class="style5">
                      <asp:Button runat="server" ID="save_btn" Text="Save Changes" OnClick="save_btnClick" style="margin-top: 32px" Width="245px"  />

                </td>
            </tr>
                         

            </table>
            
             
            <br />
         <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ErrorMessage="Please ensure your password has met all of the following requirements to be considered as a strong password:<br /><ul><li>Minimum 12 characters</li><li>At least 1 lowercase</li><li>At least 1 uppercase</li><li>At least 1 number</li><li>At least 1 special character</li></ul>" ControlToValidate="tb_newpassword" ValidationExpression="^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[$@$!%*?&])[A-Za-z\d$@$!%*?&]{12,}" ForeColor="#cc0000"></asp:RegularExpressionValidator>

        <asp:Label ID="fb_save" runat="server"></asp:Label>
             
        </div>
</asp:Content>

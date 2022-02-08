<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="PracticalAssignmentApplicationSecurity_202920H._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="jumbotron">
        <h1>
            <asp:Label ID="lb_message" runat="server" Text="Label"></asp:Label>
            <br />
            
            <br /></h1>
        <p class="lead"><asp:Label ID="lb_thank" runat="server" Text=""></asp:Label></p>
        <asp:Button ID="logout_btn" runat="server" Text="Logout" OnClick="LogoutAccount" class="btn btn-default"/>
       
    </div>

    <div>
        <p class="lead"><b><asp:Label ID="lb_fname" runat="server" Text="First Name : "></asp:Label></b><asp:Label ID="db_fname" runat="server" class="lead"></asp:Label></p>
        <p class="lead"><b><asp:Label ID="lb_lname" runat="server" Text="Last Name : "></asp:Label></b><asp:Label ID="db_lname" runat="server" class="lead"></asp:Label></p>
        <p class="lead"><b><asp:Label ID="lb_email" runat="server" Text="Email Address : "></asp:Label></b><asp:Label ID="db_email" runat="server" class="lead"></asp:Label></p>
        <p class="lead"><b><asp:Label ID="lb_credit" runat="server" Text="Credit Card : "></asp:Label></b><asp:Label ID="db_credit" runat="server" class="lead"></asp:Label></p>
        
        <p class="lead"><b><asp:Label ID="lb_dob" runat="server" Text="Date of Birth : "></asp:Label></b><asp:Label ID="db_dob" runat="server" class="lead"></asp:Label></p>
        <p class="lead"><b><asp:Label ID="lb_image" runat="server"></asp:Label></b><img id="db_image" runat="server" width="200" height="200"/></p>
        
            <%--ASP.NET</h1>--%>
        <%--<p class="lead">ASP.NET is a free web framework for building great Web sites and Web applications using HTML, CSS, and JavaScript.</p>
        <p><a href="http://www.asp.net" class="btn btn-primary btn-lg">Learn more &raquo;</a></p>
    </div>

    <div class="row">
        <div class="col-md-4">
            <h2>Getting started</h2>
            <p>
                ASP.NET Web Forms lets you build dynamic websites using a familiar drag-and-drop, event-driven model.
            A design surface and hundreds of controls and components let you rapidly build sophisticated, powerful UI-driven sites with data access.
            </p>
            <p>
                <a class="btn btn-default" href="https://go.microsoft.com/fwlink/?LinkId=301948">Learn more &raquo;</a>
            </p>
        </div>
        <div class="col-md-4">
            <h2>Get more libraries</h2>
            <p>
                NuGet is a free Visual Studio extension that makes it easy to add, remove, and update libraries and tools in Visual Studio projects.
            </p>
            <p>
                <a class="btn btn-default" href="https://go.microsoft.com/fwlink/?LinkId=301949">Learn more &raquo;</a>
            </p>
        </div>
        <div class="col-md-4">
            <h2>Web Hosting</h2>
            <p>
                You can easily find a web hosting company that offers the right mix of features and price for your applications.
            </p>
            <p>
                <a class="btn btn-default" href="https://go.microsoft.com/fwlink/?LinkId=301950">Learn more &raquo;</a>
            </p>
        </div>--%>
    </div>

</asp:Content>

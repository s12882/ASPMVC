<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="shanuMVCUserRoles.WebForm1" %>
<%@ Reference Control="Licznik.ascx">
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
          <div>
                            <UControl:UControlPerson ID="licznikLabel" runat="server" Src="~/Licznik.ascx"
                                                     Text='<% Response.Write("Current session count: " + Application["userCount"]);%>' />
                        </div>

                        <div>
                            <UControl:UControlPerson ID="UControlPerson1" runat="server" Src="~/Licznik.ascx"
                                                     Text='<% Response.Write("Current session count: " + Application["userCount"]);%>' />
                        </div>
    </form>
</body>
</html>

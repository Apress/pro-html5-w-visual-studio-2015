<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Feedback.aspx.cs" Inherits="Chapter2.Feedback" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">
        function configureRange() {
            var range = document.getElementById("Range");
            range.min = 0;
            range.max = 200;
            range.step = 20;
        }
        function updateRangeValue() {
            document.getElementById("RangeValue").value
                = document.getElementById("Range").value;
        }
    </script>
</head>
<body onload="configureRange()">
    <form id="form1" runat="server">
    <div>
        <fieldset>
            <legend>Feedback Form</legend>
            <datalist id="ticks">
                <option>0</option>
                <option>20</option>
                <option>40</option>
                <option>60</option>
                <option>80</option>
                <option>100</option>
                <option>120</option>
                <option>140</option>
                <option>160</option>
                <option>180</option>
                <option>200</option>
            </datalist>
            <ol>
                <li>
                    <asp:Label ID="lblURL" runat="server" 
                        AssociatedControlID="URL">Default home page</asp:Label>
                    <asp:textbox runat="server" ID="URL" TextMode="Url"></asp:textbox>
                </li>
                <li>
                    <asp:Label ID="lblOptions" runat="server" 
                        AssociatedControlID="Options">Default browser</asp:Label>
                    <asp:DropDownList ID="Options" runat="server">
                        <asp:ListItem Text="Internet Explorer" Value="1"></asp:ListItem>
                        <asp:ListItem Text="Google Chrome" Value="2" Selected></asp:ListItem>
                        <asp:ListItem Text="Firefox" Value="3"></asp:ListItem>
                        <asp:ListItem Text="Opera" Value="4"></asp:ListItem>
                    </asp:DropDownList>
                </li>
                <li>
                    <asp:Label ID="lblBirthday" runat="server" 
                        AssociatedControlID="Birthday">Birthday</asp:Label>
                    <asp:TextBox runat="server" ID="Birthday" TextMode="Date"></asp:TextBox>
                </li>
                <li>
                    <asp:Label ID="lblMonth" runat="server" 
                        AssociatedControlID="Month">Favorite Month</asp:Label>
                    <asp:TextBox runat="server" ID="Month" TextMode="Month"></asp:TextBox>
                </li>
                <li>
                    <asp:Label ID="lblWeek" runat="server" 
                        AssociatedControlID="Week">Busiest Week</asp:Label>
                    <asp:TextBox runat="server" ID="Week" TextMode="Week"></asp:TextBox>
                </li>
                <li>
                    <asp:Label ID="lblStart" runat="server" 
                        AssociatedControlID="DateTime">Start Date/Time</asp:Label>
                    <asp:TextBox runat="server" ID="DateTime" 
                        TextMode="DateTimeLocal"></asp:TextBox>
                </li>
                <li>
                    <asp:Label ID="lblTime" runat="server" 
                        AssociatedControlID="Time">Current Time</asp:Label>
                    <asp:TextBox runat="server" ID="Time" TextMode="Time" ></asp:TextBox>
                </li>
                <li>
                    <asp:Label ID="lblPhone" runat="server" 
                        AssociatedControlID="Phone">Phone</asp:Label>
                    <asp:TextBox runat="server" ID="Phone" TextMode="Phone"></asp:TextBox>
                </li>
                <li>
                    <asp:Label ID="lblRange" runat="server" 
                        AssociatedControlID="Range">Overall satisfaction</asp:Label>
                    <asp:TextBox runat="server" ID="Range" TextMode="Range" 
                        Width="200" Height="30" list="ticks"></asp:TextBox>
                    <asp:TextBox runat="server" ID="RangeValue" Width="50"></asp:TextBox>
                </li>
                <li>
                    <asp:Label ID="lblColor" runat="server" 
                        AssociatedControlID="Color">Preferred color</asp:Label>
                    <asp:TextBox runat="server" ID="Color" TextMode="Color"></asp:TextBox>
                </li>
                <li>
                    <asp:Label ID="lblScore" runat="server" 
                        AssociatedControlID="Score">Overall Rating</asp:Label>
                    <asp:TextBox ID="Score" runat="server" TextMode="Number" 
                        MaxLength="1"></asp:TextBox>
                </li>
                <li>
                    <asp:Label ID="lblComments" runat="server" 
                        AssociatedControlID="Multi">Comments</asp:Label>
                    <asp:TextBox runat="server" ID="Multi" TextMode="Multiline" 
                        Rows="5" Columns="30"></asp:TextBox>
                </li>
            </ol>
            <asp:Button ID="Submit" runat="server" CommandName="Submit" Text="Submit" />
        </fieldset>
    </div>
    </form>
</body>
</html>

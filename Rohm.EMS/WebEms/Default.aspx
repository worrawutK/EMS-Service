<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebEms._Default" %>

<%@ Register Assembly="System.Web.DataVisualization, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
        <div>
        <table>
        <tr>
            <td>
                <table>
                    <tr>
                        <td>PROCESS</td>
                        <td>
                            <asp:DropDownList ID="DropDownList1" runat="server">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>MACHINE</td>
                        <td>
                            <asp:DropDownList ID="DropDownList2" runat="server">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>DATE</td>
                        <td>
                            <asp:TextBox ID="RohmDateTextBox" runat="server" AutoPostBack="True"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </td>
            <td>
                <table>
                    <tr>
                        <td>
                            <asp:Chart ID="Chart1" runat="server" DataSourceID="SqlDataSource1">
                                <Series>
                                    <asp:Series Name="Series1">
                                    </asp:Series>
                                </Series>
                                <ChartAreas>
                                    <asp:ChartArea Name="ChartArea1">
                                    </asp:ChartArea>
                                </ChartAreas>
                            </asp:Chart>
                            <asp:SqlDataSource ID="SqlDataSource1" runat="server" 
                                ConnectionString="<%$ ConnectionStrings:DBxConnectionString %>" 
                                SelectCommand="SELECT TOP (1) ProcessName, MCNo, RohmDate, PlanStopLoss, StopLoss, ChokotieLoss, FreeRunningLoss, RealOperationTime, ValueOperationTime, NGLoss, MTBF, PMMTTR, PDMTTR, PerformanceRate, GoodRate, TimeOperationRate, TMERate, TotalWorkingTime, LoadTime, OperationTime, NetOperationTime, TotalGood, TotalNG, AlarmCount, BreakdownCount, BreakdownTime FROM TotalMachinEfficiencyRate"></asp:SqlDataSource>
                        </td>
                        <td>
                            <asp:Chart ID="Chart2" runat="server">
                                <Series>
                                    <asp:Series Name="Series1">
                                    </asp:Series>
                                </Series>
                                <ChartAreas>
                                    <asp:ChartArea Name="ChartArea1">
                                    </asp:ChartArea>
                                </ChartAreas>
                            </asp:Chart>
                            <asp:SqlDataSource ID="SqlDataSource2" runat="server"></asp:SqlDataSource>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:DetailsView ID="DetailsView1" runat="server" Height="50px" Width="125px">
                            </asp:DetailsView>
                            <asp:GridView ID="GridView1" runat="server">
                            </asp:GridView>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        </table>
        </div>  
    </ContentTemplate>
    </asp:UpdatePanel>

    </form>
</body>
</html>

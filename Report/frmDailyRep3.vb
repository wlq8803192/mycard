﻿
'modify code 018:
'date;2014/4/1
'auther:Hyron bjy 
'remark:通过webservice查询报表

Public Class frmDailyRep3
    Private fs_area_id As String, fs_area_type As String

    Private Sub frmDailyRep3_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ReportViewer1.ShowBackButton = False
        ReportViewer1.ShowDocumentMapButton = False
        ReportViewer1.ShowFindControls = True
        ReportViewer1.ShowRefreshButton = False

        dtpFrom.Value = DateTime.Today.AddDays(-1)

    End Sub

    Private Sub cb_query_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cb_query.Click
        'modify code 018:start-------------------------------------------------------------------------
        'Dim DB As New DataBaseReport()
        'DB.Open()
        'If Not DB.IsConnected Then
        '    frmMain.statusText.Text = "系统因连接不到数据库而无法打开窗口。请检查数据库连接。"
        '    Me.Close() : Return
        'End If
        Dim ld_date As Date = dtpFrom.Value
        Dim ld_from_month As Date = New Date(ld_date.Year, ld_date.Month, 1)

        Dim sSQL As String
        sSQL = "select r.areaenglishname as RegionName,c.areaenglishname as CityName,"
        sSQL = sSQL + "  sum(b.CardQTY) as CardQTY, sum(b.ChargedAMT) AS DailyAMT,"
        sSQL = sSQL + "  sum(rebateSalesAMT) as Discount"
        sSQL = sSQL + "  from SalesBillList as b WITH(NOLOCK) "
        sSQL = sSQL + "  join arealist as a WITH(NOLOCK) on b.storeid=a.areaid "
        sSQL = sSQL + "  join arealist as c WITH(NOLOCK) on a.parentareaid=c.areaid "
        sSQL = sSQL + "  join areascope(" + frmMain.sLoginUserID + ") as s on s.areaid=b.storeid"
        sSQL = sSQL + "  join arealist as r WITH(NOLOCK) on c.parentareaid =r.areaid"
        sSQL = sSQL + "  join arealist as t WITH(NOLOCK) on r.parentareaid =t.areaid "
        sSQL = sSQL + " where b.CULActivatedDate >= '" + ld_from_month.ToString("yyyy-MM-dd") + "' and b.CULActivatedDate <= '" + ld_date.ToString("yyyy-MM-dd") + "' "
        sSQL = sSQL + " and salesbilltype = 0  and salesbillstate < 9 "
        If fs_area_type = "H" Then
            sSQL = sSQL + ""
        ElseIf fs_area_type = "T" Then
            sSQL = sSQL + " and t.AreaID = " + fs_area_id + " "
        ElseIf fs_area_type = "R" Then
            sSQL = sSQL + " and r.AreaID = " + fs_area_id + " "
        ElseIf fs_area_type = "C" Then
            sSQL = sSQL + " and c.AreaID = " + fs_area_id + " "
        ElseIf fs_area_type = "S" Then
            sSQL = sSQL + " and a.AreaID = " + fs_area_id + " "
        End If

        sSQL = sSQL + "  group by r.areaenglishname,c.areaenglishname "

        'Dim dt As DataTable

        'dt = DB.GetDataTable(sSQL)
        'If dt Is Nothing Then
        '    frmMain.statusText.Text = "系统因在检索数据时出错而无法打开窗口。请联系软件开发人员。"
        '    DB.Close() : Me.Close() : Return
        'End If

        Dim WS As New WebService
        Dim dt As DataTable = WS.GetDataTable(sSQL)
        If dt Is Nothing Then
            frmMain.statusText.Text = "系统因在检索数据时出错而无法打开窗口。请联系软件开发人员。"
            Me.Close() : Return
        End If
        'modify code 018:end-------------------------------------------------------------------------

        Me.dsDailyRep3.tbRep.Rows.Clear()
        Dim i As Int32
        For i = 0 To dt.Rows.Count - 1
            Dim dr As DataRow
            dr = Me.dsDailyRep3.tbRep.NewRow
            dr("SalesDate") = ld_date
            dr("RegionName") = dt.Rows(i)("RegionName")
            dr("CityName") = dt.Rows(i)("CityName")
            dr("M_AMT") = dt.Rows(i)("DailyAMT")
            dr("M_CardQTY") = dt.Rows(i)("CardQTY")
            dr("M_Discount") = dt.Rows(i)("Discount")



            Dim ls_sql_row = "SELECT "
            ls_sql_row = ls_sql_row + "  sum(b.CardQTY) as CardQTY, sum(b.ChargedAMT) AS DailyAMT,"
            ls_sql_row = ls_sql_row + "  sum(rebateSalesAMT) as Discount"
            ls_sql_row = ls_sql_row + "  from SalesBillList as b WITH(NOLOCK)"
            ls_sql_row = ls_sql_row + "  join arealist as a WITH(NOLOCK) on b.storeid=a.areaid "
            ls_sql_row = ls_sql_row + "  join arealist as c WITH(NOLOCK) on a.parentareaid=c.areaid "
            ls_sql_row = ls_sql_row + "  join areascope(" + frmMain.sLoginUserID + ") as s on s.areaid=b.storeid"
            ls_sql_row = ls_sql_row + "  join arealist as r WITH(NOLOCK) on c.parentareaid =r.areaid"


            ls_sql_row = ls_sql_row + " where b.CULActivatedDate = '" + ld_date.ToString("yyyy-MM-dd") + "'"
            ls_sql_row = ls_sql_row + " and b.salesbilltype = 0  and b.salesbillstate < 9 "
            ls_sql_row = ls_sql_row + " and r.areaenglishname='" + dr("RegionName").ToString() + "' "
            ls_sql_row = ls_sql_row + " and c.areaenglishname='" + dr("CityName").ToString() + "' "

            'modify code 018:start-------------------------------------------------------------------------
            'Dim dt1 As DataTable
            'dt1 = DB.GetDataTable(ls_sql_row)
            Dim dt1 As DataTable = WS.GetDataTable(ls_sql_row)
            'modify code 018:end-------------------------------------------------------------------------
            If dt1.Rows.Count > 0 Then
                dr("DailyAMT") = dt1.Rows(0)("DailyAMT")
                dr("CardQTY") = dt1.Rows(0)("CardQTY")
                dr("Discount") = dt1.Rows(0)("Discount")
            End If

            Me.dsDailyRep3.tbRep.Rows.Add(dr)
        Next

        'modify code 018:start-------------------------------------------------------------------------
        'DB.Close()
        'modify code 018:end-------------------------------------------------------------------------
        Me.ReportViewer1.RefreshReport()
        ReportingLog.InsertLog("系统报表", Me.Text, sSQL)
    End Sub

    Private Sub Panel1_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Panel1.Paint

    End Sub

    Private Sub btnSelect_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSelect.Click
        frmSelArea.fs_showlevel = "NoStore"
        If frmSelArea.ShowDialog() = Windows.Forms.DialogResult.OK Then
            txtArea.Text = frmSelArea.trvArea.SelectedNode.Text
            fs_area_id = frmSelArea.trvArea.SelectedNode.Name
            fs_area_type = frmSelArea.trvArea.SelectedNode.ImageKey
        End If

    End Sub

    Private Sub txtArea_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtArea.TextChanged

    End Sub

    Private Sub Label3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label3.Click

    End Sub
End Class
Imports System.Windows.Forms
Imports System.Drawing
Imports System.ComponentModel
Imports System.Reflection

Public Class CustomCalendar
    Inherits UserControl

    Private _currentMonth As Date = Date.Now
    Private _deadlineDates As New List(Of Date)
    Private _selectedDate As Date = Date.Now.Date

    Public Event DateSelected(selectedDate As Date)
    Public Event MonthChanged(newMonth As Date)

    Private daysGrid As TableLayoutPanel
    Private lblMonthYear As Label
    Private btnPrev As Button
    Private btnNext As Button
    Private pnlPicker As Panel
    Private pnlMonthsView As Panel
    Private pnlYearsView As Panel
    Private lblPickerYear As Label
    Private _tempYear As Integer
    Private _monthButtons As New List(Of Button)

    Public Sub New()
        Me.Size = New Size(400, 350)
        Me.BackColor = Color.White
        Me.DoubleBuffered = True

        Dim headerPanel As New Panel()
        headerPanel.Dock = DockStyle.Top
        headerPanel.Height = 50

        lblMonthYear = New Label()
        lblMonthYear.Dock = DockStyle.Fill
        lblMonthYear.TextAlign = ContentAlignment.MiddleCenter
        lblMonthYear.Font = New Font("Segoe UI", 14, FontStyle.Bold)
        lblMonthYear.ForeColor = Color.FromArgb(44, 62, 80)
        lblMonthYear.Cursor = Cursors.Hand

        AddHandler lblMonthYear.MouseEnter, Sub() lblMonthYear.ForeColor = Color.FromArgb(52, 152, 219)
        AddHandler lblMonthYear.MouseLeave, Sub() lblMonthYear.ForeColor = Color.FromArgb(44, 62, 80)

        AddHandler lblMonthYear.Click, AddressOf lblMonthYear_Click

        btnPrev = New Button()
        btnPrev.Text = "<"
        btnPrev.Dock = DockStyle.Left
        btnPrev.Width = 40
        btnPrev.FlatStyle = FlatStyle.Flat
        btnPrev.FlatAppearance.BorderSize = 0
        btnPrev.ForeColor = Color.FromArgb(52, 152, 219)
        btnPrev.Font = New Font("Segoe UI", 12, FontStyle.Bold)

        btnNext = New Button()
        btnNext.Text = ">"
        btnNext.Dock = DockStyle.Right
        btnNext.Width = 40
        btnNext.FlatStyle = FlatStyle.Flat
        btnNext.FlatAppearance.BorderSize = 0
        btnNext.ForeColor = Color.FromArgb(52, 152, 219)
        btnNext.Font = New Font("Segoe UI", 12, FontStyle.Bold)

        AddHandler btnPrev.Click, Sub() ChangeMonth(-1)
        AddHandler btnNext.Click, Sub() ChangeMonth(1)

        headerPanel.Controls.Add(lblMonthYear)
        headerPanel.Controls.Add(btnPrev)
        headerPanel.Controls.Add(btnNext)

        Dim daysHeader As New TableLayoutPanel()
        daysHeader.Dock = DockStyle.Top
        daysHeader.Height = 30
        daysHeader.ColumnCount = 7
        daysHeader.RowCount = 1

        For i As Integer = 0 To 6
            daysHeader.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 14.28F))
        Next

        Dim dayNames() As String = {"Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"}
        For i As Integer = 0 To 6
            Dim lblDayName As New Label()
            lblDayName.Text = dayNames(i)
            lblDayName.TextAlign = ContentAlignment.MiddleCenter
            lblDayName.Dock = DockStyle.Fill
            lblDayName.Font = New Font("Segoe UI", 9, FontStyle.Bold)
            lblDayName.ForeColor = Color.Gray
            daysHeader.Controls.Add(lblDayName, i, 0)
        Next

        daysGrid = New TableLayoutPanel()
        daysGrid.Dock = DockStyle.Fill
        daysGrid.ColumnCount = 7
        daysGrid.RowCount = 6
        EnableDoubleBuffered(daysGrid)

        For i As Integer = 0 To 6
            daysGrid.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 14.28F))
        Next
        For i As Integer = 0 To 5
            daysGrid.RowStyles.Add(New RowStyle(SizeType.Percent, 16.66F))
        Next

        Me.Controls.Add(daysGrid)
        Me.Controls.Add(daysHeader)
        Me.Controls.Add(headerPanel)

        SetupPickerPanel()
        RenderCalendar()
    End Sub

    Private Sub SetupPickerPanel()
        pnlPicker = New Panel()
        pnlPicker.Size = Me.Size
        pnlPicker.BackColor = Color.White
        pnlPicker.Location = New Point(0, 0)
        pnlPicker.Visible = False
        pnlPicker.BorderStyle = BorderStyle.None
        EnableDoubleBuffered(pnlPicker)

        ' Header
        Dim headerPanel As New Panel()
        headerPanel.Dock = DockStyle.Top
        headerPanel.Height = 50
        headerPanel.Padding = New Padding(15, 0, 15, 0)

        lblPickerYear = New Label()
        lblPickerYear.AutoSize = True
        lblPickerYear.Font = New Font("Segoe UI", 16, FontStyle.Bold)
        lblPickerYear.ForeColor = Color.FromArgb(44, 62, 80)
        lblPickerYear.Cursor = Cursors.Hand
        lblPickerYear.Location = New Point(15, 10)
        AddHandler lblPickerYear.Click, Sub() ShowYearsView()
        headerPanel.Controls.Add(lblPickerYear)

        Dim btnClosePicker As New Button()
        btnClosePicker.Text = "×"
        btnClosePicker.Size = New Size(40, 40)
        btnClosePicker.Location = New Point(pnlPicker.Width - 45, 5)
        btnClosePicker.FlatStyle = FlatStyle.Flat
        btnClosePicker.FlatAppearance.BorderSize = 0
        btnClosePicker.Font = New Font("Segoe UI", 20)
        btnClosePicker.ForeColor = Color.FromArgb(149, 165, 166)
        btnClosePicker.Cursor = Cursors.Hand
        AddHandler btnClosePicker.Click, Sub() pnlPicker.Visible = False
        headerPanel.Controls.Add(btnClosePicker)

        pnlPicker.Controls.Add(headerPanel)

        ' Content Container
        Dim contentPanel As New Panel()
        contentPanel.Dock = DockStyle.Fill
        pnlPicker.Controls.Add(contentPanel)
        contentPanel.BringToFront()

        ' Months View
        pnlMonthsView = New Panel()
        pnlMonthsView.Dock = DockStyle.Fill
        EnableDoubleBuffered(pnlMonthsView)
        contentPanel.Controls.Add(pnlMonthsView)

        Dim monthsGrid As New TableLayoutPanel()
        monthsGrid.Dock = DockStyle.Fill
        monthsGrid.ColumnCount = 3
        monthsGrid.RowCount = 4
        monthsGrid.Padding = New Padding(10)
        For i As Integer = 0 To 2 : monthsGrid.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 33.33F)) : Next
        For i As Integer = 0 To 3 : monthsGrid.RowStyles.Add(New RowStyle(SizeType.Percent, 25.0F)) : Next

        Dim months() As String = {"January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"}
        _monthButtons.Clear()
        For i As Integer = 0 To 11
            Dim monthNum As Integer = i + 1
            Dim btnMonth As New Button()
            btnMonth.Text = months(i)
            btnMonth.Dock = DockStyle.Fill
            btnMonth.FlatStyle = FlatStyle.Flat
            btnMonth.FlatAppearance.BorderSize = 0
            btnMonth.Font = New Font("Segoe UI", 11)
            btnMonth.Cursor = Cursors.Hand
            btnMonth.Tag = monthNum
            btnMonth.Margin = New Padding(2)

            AddHandler btnMonth.Click, Sub(s, ev)
                                           Dim m = CInt(DirectCast(s, Button).Tag)
                                           _currentMonth = New Date(_tempYear, m, 1)
                                           pnlPicker.Visible = False
                                           RenderCalendar()
                                           RaiseEvent MonthChanged(_currentMonth)
                                       End Sub
            monthsGrid.Controls.Add(btnMonth, i Mod 3, i \ 3)
            _monthButtons.Add(btnMonth)
        Next
        pnlMonthsView.Controls.Add(monthsGrid)

        ' Years View
        pnlYearsView = New Panel()
        pnlYearsView.Dock = DockStyle.Fill
        pnlYearsView.Visible = False
        EnableDoubleBuffered(pnlYearsView)
        contentPanel.Controls.Add(pnlYearsView)

        Dim yearsScroll As New Panel()
        yearsScroll.Dock = DockStyle.Fill
        yearsScroll.AutoScroll = True
        pnlYearsView.Controls.Add(yearsScroll)

        Dim yearsGrid As New TableLayoutPanel()
        yearsGrid.Dock = DockStyle.Top
        yearsGrid.ColumnCount = 3
        yearsGrid.AutoSize = True
        yearsGrid.Padding = New Padding(10)
        yearsGrid.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 33.33F))
        yearsGrid.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 33.33F))
        yearsGrid.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 33.33F))

        Dim currentYear As Integer = Date.Now.Year
        Dim startYear As Integer = currentYear - 10
        Dim endYear As Integer = currentYear + 11 ' Show 22 years total

        For i As Integer = 0 To 21
            Dim yearNum As Integer = startYear + i
            Dim btnYear As New Button()
            btnYear.Text = yearNum.ToString()
            btnYear.Height = 40
            btnYear.Dock = DockStyle.Fill
            btnYear.FlatStyle = FlatStyle.Flat
            btnYear.FlatAppearance.BorderSize = 0
            btnYear.Font = New Font("Segoe UI", 11)
            btnYear.Cursor = Cursors.Hand
            btnYear.Tag = yearNum
            btnYear.Margin = New Padding(2)

            AddHandler btnYear.Click, Sub(s, ev)
                                          _tempYear = CInt(DirectCast(s, Button).Tag)
                                          ShowMonthsView()
                                      End Sub

            yearsGrid.Controls.Add(btnYear, i Mod 3, i \ 3)
        Next
        yearsScroll.Controls.Add(yearsGrid)

        Me.Controls.Add(pnlPicker)
        pnlPicker.BringToFront()
    End Sub

    Private Sub ShowMonthsView()
        lblPickerYear.Text = _tempYear.ToString()
        pnlYearsView.Visible = False
        pnlMonthsView.Visible = True

        ' Highlight current month in temp view
        For Each btn In _monthButtons
            Dim mNum = CInt(btn.Tag)
            If mNum = _currentMonth.Month AndAlso _tempYear = _currentMonth.Year Then
                btn.ForeColor = Color.FromArgb(52, 152, 219)
                btn.Font = New Font("Segoe UI", 11, FontStyle.Bold)
            Else
                btn.ForeColor = Color.FromArgb(44, 62, 80)
                btn.Font = New Font("Segoe UI", 11, FontStyle.Regular)
            End If
        Next
    End Sub

    Private Sub ShowYearsView()
        pnlMonthsView.Visible = False
        pnlYearsView.Visible = True
    End Sub

    <DesignerSerializationVisibility(DesignerSerializationVisibility.Content)>
    Public Property DeadlineDates As List(Of Date)
        Get
            Return _deadlineDates
        End Get
        Set(value As List(Of Date))
            _deadlineDates = value
            RenderCalendar()
        End Set
    End Property

    Public Sub SetDate(targetDate As Date)
        _currentMonth = New Date(targetDate.Year, targetDate.Month, 1)
        _selectedDate = targetDate.Date
        RenderCalendar()
    End Sub

    Public Function GetDate() As Date
        Return _currentMonth
    End Function

    Private Sub ChangeMonth(diff As Integer)
        _currentMonth = _currentMonth.AddMonths(diff)
        RenderCalendar()
        RaiseEvent MonthChanged(_currentMonth)
    End Sub

    Private Sub RenderCalendar()
        If daysGrid Is Nothing Then Exit Sub

        daysGrid.SuspendLayout()
        lblMonthYear.Text = _currentMonth.ToString("MMMM yyyy")

        Dim firstDayOfMonth As New Date(_currentMonth.Year, _currentMonth.Month, 1)
        Dim startDayOfWeek As Integer = CInt(firstDayOfMonth.DayOfWeek)
        Dim startDate As Date = firstDayOfMonth.AddDays(-startDayOfWeek)

        ' Ensure we have 42 labels
        If daysGrid.Controls.Count <> 42 Then
            daysGrid.Controls.Clear()
            For i As Integer = 0 To 41
                Dim lblDay As New Label()
                lblDay.Width = 40
                lblDay.Height = 40
                lblDay.Anchor = AnchorStyles.None
                lblDay.TextAlign = ContentAlignment.MiddleCenter
                lblDay.Cursor = Cursors.Hand
                lblDay.Font = New Font("Segoe UI", 10)
                AddHandler lblDay.Click, AddressOf Day_Click
                AddHandler lblDay.MouseEnter, AddressOf Day_MouseEnter
                AddHandler lblDay.MouseLeave, AddressOf Day_MouseLeave
                AddHandler lblDay.Paint, AddressOf Day_Paint
                daysGrid.Controls.Add(lblDay, i Mod 7, i \ 7)
            Next
        End If

        ' Update labels
        For i As Integer = 0 To 41
            Dim lblDay = DirectCast(daysGrid.Controls(i), Label)
            Dim currentDayDate As Date = startDate.AddDays(i)
            Dim isOtherMonth As Boolean = (currentDayDate.Month <> _currentMonth.Month)

            lblDay.Text = currentDayDate.Day.ToString()
            lblDay.Tag = currentDayDate

            ' Reset state
            lblDay.BackColor = Color.Transparent
            lblDay.Region = Nothing

            ' Logic for colors
            Dim isDeadline As Boolean = _deadlineDates.Contains(currentDayDate)
            Dim isSelected As Boolean = (currentDayDate = _selectedDate)
            Dim isToday As Boolean = (currentDayDate = Date.Now.Date)

            ' Base colors
            If isSelected Then
                lblDay.BackColor = Color.FromArgb(52, 152, 219)
                lblDay.ForeColor = Color.White
                lblDay.Font = New Font("Segoe UI", 10, FontStyle.Bold)

                Dim path As New System.Drawing.Drawing2D.GraphicsPath()
                path.AddEllipse(0, 0, lblDay.Width, lblDay.Height)
                lblDay.Region = New Region(path)
            Else
                If isOtherMonth Then
                    lblDay.ForeColor = Color.FromArgb(189, 195, 199)
                Else
                    lblDay.ForeColor = Color.FromArgb(44, 62, 80)
                End If

                If isToday Then
                    lblDay.ForeColor = Color.FromArgb(52, 152, 219)
                    lblDay.Font = New Font("Segoe UI", 10, FontStyle.Bold)
                ElseIf isDeadline AndAlso Not isOtherMonth Then
                    lblDay.ForeColor = Color.FromArgb(231, 76, 60)
                    lblDay.Font = New Font("Segoe UI", 10, FontStyle.Bold)
                Else
                    lblDay.Font = New Font("Segoe UI", 10)
                End If
            End If

            ' Redraw to trigger Paint event for dots
            lblDay.Invalidate()
        Next

        daysGrid.ResumeLayout()
    End Sub

    Private Sub Day_MouseEnter(sender As Object, e As EventArgs)
        Dim l = DirectCast(sender, Label)
        If DirectCast(l.Tag, Date) <> _selectedDate Then
            l.BackColor = Color.FromArgb(240, 243, 244)
            Dim path As New System.Drawing.Drawing2D.GraphicsPath()
            path.AddEllipse(0, 0, l.Width, l.Height)
            l.Region = New Region(path)
        End If
    End Sub

    Private Sub Day_MouseLeave(sender As Object, e As EventArgs)
        Dim l = DirectCast(sender, Label)
        If DirectCast(l.Tag, Date) <> _selectedDate Then
            l.BackColor = Color.Transparent
            l.Region = Nothing
        End If
    End Sub

    Private Sub Day_Paint(sender As Object, e As PaintEventArgs)
        Dim lbl = DirectCast(sender, Label)
        Dim currentDayDate = DirectCast(lbl.Tag, Date)
        Dim isOtherMonth As Boolean = (currentDayDate.Month <> _currentMonth.Month)
        Dim isDeadline As Boolean = _deadlineDates.Contains(currentDayDate)
        Dim isSelected As Boolean = (currentDayDate = _selectedDate)

        If isDeadline AndAlso Not isOtherMonth Then
            Dim dotSize As Integer = 4
            Dim brush As New SolidBrush(If(isSelected, Color.White, Color.FromArgb(231, 76, 60)))
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias
            e.Graphics.FillEllipse(brush, (lbl.Width \ 2) - (dotSize \ 2), lbl.Height - 6, dotSize, dotSize)
            brush.Dispose()
        End If
    End Sub

    Private Sub Day_Click(sender As Object, e As EventArgs)
        Dim lbl = DirectCast(sender, Label)
        Dim clickedDate As Date = DirectCast(lbl.Tag, Date)

        ' If user clicks a date from another month, switch to that month
        If clickedDate.Month <> _currentMonth.Month OrElse clickedDate.Year <> _currentMonth.Year Then
            _currentMonth = New Date(clickedDate.Year, clickedDate.Month, 1)
            RaiseEvent MonthChanged(_currentMonth)
        End If

        _selectedDate = clickedDate
        RenderCalendar()
        RaiseEvent DateSelected(_selectedDate)
    End Sub

    Private Sub lblMonthYear_Click(sender As Object, e As EventArgs)
        _tempYear = _currentMonth.Year
        ShowMonthsView()
        pnlPicker.Visible = True
        pnlPicker.BringToFront()
    End Sub

    Private Sub EnableDoubleBuffered(control As Control)
        Dim propertyInfo = control.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance Or BindingFlags.NonPublic)
        If propertyInfo IsNot Nothing Then
            propertyInfo.SetValue(control, True, Nothing)
        End If
    End Sub
End Class

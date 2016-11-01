' Generic Printer
Public Class GenericPrinter

    ' Events
    Public Event SendMessage(ByVal Message As String)

    ''' <summary>
    ''' Collects the default printer information
    ''' </summary>
    ''' <returns>PrinterStructure</returns>
    ''' <remarks></remarks>
    Public Function Collect() As PrinterStructure
        Dim printer_structure As New PrinterStructure

        ' Set Debug Propertoes
        printer_structure.debug_time_start = DateTime.Now

        ' Try to connect to the device
        If ValidatePrinter() = False Then
            ' Stop the scan
            Return printer_structure ' Will return blank
        End If

        ' We have a valid printer, lets set our timeout to longer
        printer_structure.information_is_printer = True
        device_snmp_timeout = 2000
        ' We have scanned some information now
        printer_structure.information_was_scanned = True

        ' Get General Information From the Printer
        printer_structure.information_ip_address = Net.IPAddress.Parse(device_ip_address)
        printer_structure.information_community_string = device_snmp_password
        ' System Object Identifier
        printer_structure.information_system_identifier = getSysObjectId()
        ' Engine
        printer_structure.information_engine = getEngine(printer_structure.information_system_identifier)
        ' Manufacture
        'printer_structure.general_manufacture = getManufacture()
        printer_structure.general_manufacture = printer_structure.information_engine
        ' Model Number
        printer_structure.general_model = getModel()
        ' Serial Number
        printer_structure.general_serial = getSerialNumber()
        ' Is Color Device
        printer_structure.general_color = getColor()

        ' Get Toner Supplies From The Printer
        ' Black Toner
        printer_structure.supplies_black = getToner("black")
        ' Cyan Toner
        printer_structure.supplies_cyan = getToner("cyan")
        ' Magenta Toner
        printer_structure.supplies_magenta = getToner("magenta")
        ' Yellow Toner
        printer_structure.supplies_yellow = getToner("yellow")

        ' Second Validation for Colour
        If printer_structure.supplies_cyan.Index > 0 Or printer_structure.supplies_magenta.Index > 0 Or printer_structure.supplies_yellow.Index > 0 Then
            printer_structure.general_color = True
        End If


        ' Get Counter Information From the Printer
        ' Total Count
        printer_structure.counter_total = getTotalCount()
        ' Get Mono (if mono printer)
        If printer_structure.general_color = False Then
            printer_structure.counter_mono_total = printer_structure.counter_total
        End If

        Return printer_structure
    End Function

    Function ValidatePrinter() As Boolean
        RaiseEvent SendMessage("Validating Printer")

        Dim valid As Boolean = False
        Dim response As String = getNextSNMP("1") ' Call the SNMP but check to see if it can connect via IP first

        If response.Trim.Length > 0 Then
            valid = True
        End If

        Return valid
    End Function

    Function getSysObjectId(Optional ByVal s_oid = "1.3.6.1.2.1.25.3.2.1.4", Optional ByVal bLastTry = False) As String
        RaiseEvent SendMessage("System Object Identifier")

        Dim sysObjectId As String = getNextSNMP(s_oid)

        If sysObjectId.Replace("0", "").Replace(".", "").Trim.Length <= 0 And bLastTry = False Then
            ' incomplete oids, check another default one
            Dim tmpSystemObjectId = sysObjectId.Replace("0", "").Replace(".", "").Trim
            If tmpSystemObjectId = "" Or tmpSystemObjectId = "0" Or tmpSystemObjectId = "1" Or tmpSystemObjectId = "2" Then
                sysObjectId = getSysObjectId("1.3.6.1.2.1.1.2", True)
            End If
        End If

        sysObjectId = sysObjectId.Replace("1.3.6.1.4.1.", "")
        sysObjectId = sysObjectId.Replace("0.", "") ' this is new

        If sysObjectId.IndexOf(".") >= 0 Then
            sysObjectId = sysObjectId.Substring(0, sysObjectId.IndexOf("."))
        End If

        Return sysObjectId
    End Function

    Function getManufacture() As String
        RaiseEvent SendMessage("Manufacturer")

        Dim manufacture As String = getNextSNMP("1.3.6.1.2.1.43.8.2.1.14")

        Return manufacture
    End Function

    Function getModel() As String
        RaiseEvent SendMessage("Model")

        Dim model As String = getSNMP("1.3.6.1.2.1.25.3.2.1.3.1")

        Return model
    End Function

    Function getSerialNumber() As String
        RaiseEvent SendMessage("Serial Number")

        Dim serial_number As String = getNextSNMP("1.3.6.1.2.1.43.5.1.1.17")
        ' Was: 1.3.6.1.2.1.43.8.2.1.17

        Return serial_number
    End Function

    Function getColor() As Boolean
        RaiseEvent SendMessage("Color")

        Dim color As Boolean = False
        Dim color_numbers As String = getNextSNMP("1.3.6.1.2.1.43.10.2.1.6")

        If Val(color_numbers) > 1 Then
            color = True
        End If

        Return color
    End Function

    Function getTotalCount() As String
        RaiseEvent SendMessage("Color Count")

        Dim count As String = getNextSNMP("1.3.6.1.2.1.43.10.2.1.4")

        Return count
    End Function

    Function getToner(ByVal color As String) As TonerStructure
        RaiseEvent SendMessage("Toner " & color)

        Dim toner_oid As String = "1.3.6.1.2.1.43.12.1.1.4.1."
        Dim toner As New TonerStructure
        Dim oid_index As Integer = 1
        Dim toner_found As Boolean = False

        ' Cycle until color is found
        Do While toner_found = False
            Dim toner_color As String = getSNMP(toner_oid & oid_index)

            ' Toner was not found
            If toner_color.Trim.Length <= 0 Then
                toner_found = True
                oid_index = 0
            Else
                ' Check to see if it is the color we want
                If InStr(toner_color.Trim.ToLower, color.Trim.ToLower, CompareMethod.Text) > 0 Then
                    toner_found = True
                Else
                    ' Set New Oid Index
                    oid_index += 1
                End If
            End If
        Loop

        ' Validate Object Id
        If oid_index > 0 Then
            ' Set Index
            toner.Index = oid_index
            ' Grab Part
            toner.Part = getSNMP("1.3.6.1.2.1.43.11.1.1.6.1." & oid_index)
            ' Grab Maximum
            toner.Maximum_Level = getSNMP("1.3.6.1.2.1.43.11.1.1.8.1." & oid_index)
            ' Grab Current
            toner.Current_Level = getSNMP("1.3.6.1.2.1.43.11.1.1.9.1." & oid_index)
            ' Calculate Percentage
            toner.Percentage = "0"
            If Val(toner.Current_Level) > 0 And Val(toner.Maximum_Level) > 0 Then
                toner.Current_Level = Val(toner.Current_Level)
                toner.Maximum_Level = Val(toner.Maximum_Level)

                toner.Percentage = Math.Round((toner.Current_Level / toner.Maximum_Level) * 100, 2)
            End If
        End If

        Return toner
    End Function

    Function getEngine(ByVal sysObjectId As String) As String
        RaiseEvent SendMessage("Engine")

        Dim manufacture As String

        Select Case Val(sysObjectId.Trim)
            Case "2435"
                manufacture = "Brother"
            Case "1602"
                manufacture = "Canon"
            Case "674"
                manufacture = "Dell"
            Case "236"
                manufacture = "Samsung"
            Case "1248"
                manufacture = "Epson"
            Case "11"
                manufacture = "Hewlett Packard"
            Case "641"
                manufacture = "Lexmark"
            Case "2001"
                manufacture = "Oki Data Corporation"
            Case "367"
                manufacture = "Ricoh"
            Case "253"
                manufacture = "Xerox"
            Case Else
                manufacture = "Unknown"
        End Select

        Return manufacture
    End Function

    Function getDisplay() As String
        Dim line1 As String = getSNMP("1.3.6.1.2.1.43.16.5.1.2.1.1")
        Dim line2 As String = getSNMP("1.3.6.1.2.1.43.16.5.1.2.1.2")
        Dim line3 As String = getSNMP("1.3.6.1.2.1.43.16.5.1.2.1.3")
        Dim line4 As String = getSNMP("1.3.6.1.2.1.43.16.5.1.2.1.4")
        Dim display As String = ""

        If line1.Length > 0 Then
            display = If(display.Length > 0, ", ", "") & line1
        End If
        If line2.Length > 0 Then
            display = If(display.Length > 0, ", ", "") & line2
        End If
        If line3.Length > 0 Then
            display = If(display.Length > 0, ", ", "") & line3
        End If
        If line4.Length > 0 Then
            display = If(display.Length > 0, ", ", "") & line4
        End If

        Return display

    End Function

End Class

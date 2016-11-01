Public Class Scan

    Public Sub New(ByVal ip_address As Net.IPAddress, Optional ByVal snmp_password As String = "public", Optional ByVal timeout As Integer = 50)

        device_ip_address = ip_address.ToString
        device_snmp_password = snmp_password
        device_snmp_timeout = timeout
    End Sub

    Public Function collect() As PrinterStructure

        Dim default_printer As New GenericPrinter
        Dim printer_structure As New PrinterStructure

        printer_structure = default_printer.Collect()
        If printer_structure.information_was_scanned = True Then
            printer_structure = engineScan(printer_structure)
        End If

        ' We are finished so add the time
        printer_structure.debug_time_start = DateTime.Now

        Return printer_structure
    End Function

    Function engineScan(ByVal ps As PrinterStructure) As PrinterStructure
        Dim printer_structure As New PrinterStructure
        printer_structure = ps

        Select Case printer_structure.information_engine
            Case "Canon"
                Dim CanonPrinter As New CanonPrinter
                printer_structure = CanonPrinter.collect(printer_structure)
            Case "Dell"
                Dim DellPrinter As New DellPrinter
                printer_structure = DellPrinter.collect(printer_structure)
            Case "Hewlett Packard"
                Dim HewlettPackardPrinter As New HewlettPackardPrinter
                printer_structure = HewlettPackardPrinter.collect(printer_structure)
            Case "Lexmark"
                Dim LexmarkPrinter As New LexmarkPrinter
                printer_structure = LexmarkPrinter.collect(printer_structure)
            Case "Ricoh"
                Dim RicohPrinter As New RicohPrinter
                printer_structure = RicohPrinter.collect(printer_structure)
            Case "Xerox"
                Dim XeroxPrinter As New XeroxPrinter
                printer_structure = XeroxPrinter.collect(printer_structure)
            Case Else
                ' Do Nothing
        End Select

        Return printer_structure
    End Function

End Class

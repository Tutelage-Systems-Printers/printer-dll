Public Class LexmarkPrinter

    Public Function collect(ByVal ps As PrinterStructure) As PrinterStructure
        Dim printer_structure As New PrinterStructure
        Dim bSpecificPrinter As Boolean = False

        printer_structure = ps

        ' Grab Printer Defaults
        Dim printer_serial As String = getSerialNumber()
        If printer_serial.Length > 0 Then
            printer_structure.general_serial = printer_serial
        End If

        Return printer_structure
    End Function

    Function getSerialNumber() As String
        Dim serial_number As String = getNextSNMP("1.3.6.1.4.1.641.2.1.2.1.6")

        Return serial_number
    End Function

End Class

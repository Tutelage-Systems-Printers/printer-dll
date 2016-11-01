Public Class RicohPrinter

    Public Function collect(ByVal ps As PrinterStructure) As PrinterStructure
        Dim printer_structure As New PrinterStructure

        printer_structure = ps

        ' Grab Total Count
        printer_structure.counter_total = getOid("1.3.6.1.4.1.367.3.2.1.2.19.2.0")
        ' Grab Mono Count
        printer_structure.counter_mono_total = getOid("1.3.6.1.4.1.367.3.2.1.2.19.5.1.9.20")

        ' Grab Printer Defaults
        Dim printer_serial As String = getSerialNumber()
        If printer_serial.Length > 0 Then
            printer_structure.general_serial = printer_serial
        End If

        If printer_structure.general_color Then
            ' if the device is a color device, let's cheat and subtract the mono from the total
            printer_structure.counter_color_total = printer_structure.counter_total - printer_structure.counter_mono_total
        End If

        Return printer_structure
    End Function

    Function getSerialNumber() As String
        Dim serial_number As String = getNextSNMP("1.3.6.1.4.1.367.3.2.1.2.1.4")

        Return serial_number
    End Function

    Function getOid(oid As String) As String
        Return getSNMP(oid)
    End Function

End Class

Public Class DellPrinter

    Public Function collect(ByVal ps As PrinterStructure) As PrinterStructure
        Dim printer_structure As New PrinterStructure

        printer_structure = ps

        ' Grab Default Dell Settings
        ' Model
        printer_structure.general_model = getModel()
        ' Serial Number
        printer_structure.general_serial = getSerial()

        Return printer_structure
    End Function

    Function getModel() As String
        Dim model As String = getNextSNMP("1.3.6.1.4.1.641.2.1.2.1.2")

        Return model
    End Function

    Function getSerial() As String
        Dim serial_number As String = getNextSNMP("1.3.6.1.4.1.641.2.1.2.1.6")

        Return serial_number
    End Function

End Class

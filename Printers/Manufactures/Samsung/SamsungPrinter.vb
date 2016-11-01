Module SamsungPrinter

    Public Function collect(ByVal ps As PrinterStructure) As PrinterStructure
        Dim printer_structure As New PrinterStructure

        printer_structure = ps

        ' Grab Printer Defaults
        printer_structure.general_model = getModel()

        Return printer_structure
    End Function

    Function getModel() As String
        Return getNextSNMP("1.3.6.1.4.1.236.11.5.1.1.1.1")
    End Function

End Module

Public Class Xerox_6000

    Public Function collect(ByVal ps As PrinterStructure) As PrinterStructure
        Dim printer_structure As New PrinterStructure

        printer_structure = ps

        ' Model Specific
        If printer_structure.general_model.ToUpper.Contains("PHASER 6180") Then
            get6180(printer_structure)
        End If


        Return printer_structure
    End Function

    Sub get6180(ByRef printer_structure As PrinterStructure)
        ' We know this is a colour device
        printer_structure.general_color = True
        printer_structure.counter_mono_total = getSNMP("1.3.6.1.4.1.253.8.53.13.2.1.6.1.20.34")
        printer_structure.counter_color_total = getSNMP("1.3.6.1.4.1.253.8.53.13.2.1.6.1.20.33")
    End Sub

    Function getCount() As String
        Dim mono_counter As String = getNextSNMP("1.3.6.1.4.1.253.8.53.13.2.1.6.1.20.34")

        Return mono_counter
    End Function

End Class

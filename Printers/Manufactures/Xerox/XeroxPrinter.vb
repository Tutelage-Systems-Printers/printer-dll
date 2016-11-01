Public Class XeroxPrinter

    Public Function collect(ByVal ps As PrinterStructure) As PrinterStructure
        Dim printer_structure As New PrinterStructure
        Dim bSpecificPrinter As Boolean = False

        printer_structure = ps

        ' Model Specific
        If printer_structure.general_model.ToUpper.IndexOf("PHASER 6180") > 0 Then
            Dim Series6000 As New Xerox_6000

            printer_structure = Series6000.collect(printer_structure)

            bSpecificPrinter = True
        End If

        If bSpecificPrinter = False Then
            getDefaultDevice(printer_structure)
        End If


        Return printer_structure
    End Function

    Sub getDefaultDevice(ByRef printer_structure As PrinterStructure)
        ' Grab Default Hewlett Packard Settings
        Dim mono_counter As String = getMonoCount()
        Dim color_counter As String = getColorCount()

        ' Validation
        printer_structure.counter_mono_total = mono_counter

        If color_counter > 0 And printer_structure.general_color = True Then
            printer_structure.counter_color_total = getColorCount()
        Else
            ' Default Black Printer
            printer_structure.counter_mono_total = printer_structure.counter_total
        End If
    End Sub

    Function getMonoCount() As String
        Dim mono_counter As String = getNextSNMP("1.3.6.1.4.1.253.8.53.13.2.1.6.1.20.34")

        Return mono_counter
    End Function

    Function getColorCount() As String
        Dim color_counter As String = getNextSNMP("1.3.6.1.4.1.253.8.53.13.2.1.6.1.20.33")

        Return color_counter
    End Function

End Class

Public Class HewlettPackardPrinter

    Public Function collect(ByVal ps As PrinterStructure) As PrinterStructure
        Dim printer_structure As PrinterStructure = ps
        Dim hasAlreadyCollected As Boolean = False

        ' We have to look at the model number and/or the general model itself
        ' to figure out if we need to run the custom device collection

        ' First let's try the model number
        Dim model_number As String = getModelNumber()

        Select Case model_number
            Case "CQ521A"
                'Photosmart
                hasAlreadyCollected = True
                printer_structure = HPPhotosmart.collect(printer_structure)
            Case "A7F64A"
                ' Office Jet
                hasAlreadyCollected = True
                printer_structure = HPOfficejetPrinter.collect(printer_structure)
        End Select

        ' Now lets try the model to narrow it down
        If printer_structure.general_model.ToLower.IndexOf("officejet") >= 0 Then
            If hasAlreadyCollected = False Then
                hasAlreadyCollected = True
                printer_structure = HPOfficejetPrinter.collect(printer_structure)
            End If
        End If

        If printer_structure.general_model.ToLower.IndexOf("photosmart") >= 0 Then
            If hasAlreadyCollected = False Then
                hasAlreadyCollected = True
                printer_structure = HPPhotosmart.collect(printer_structure)
            End If
        End If

        If hasAlreadyCollected = False Then
            ' Grab Default Hewlett Packard Settings

            ' Take a guess at the serial number
            If printer_structure.general_serial.Length <= 3 Then
                printer_structure.general_serial = getSerialNumber()
            End If
        End If

        Return printer_structure
    End Function

    Function getColor() As Boolean
        Dim color As Boolean = False
        Dim color_numbers As String = getNextSNMP("1.3.6.1.4.1.11.2.3.9.4.2.2.10.2.1.6")

        If Val(color_numbers) > 1 Then
            color = True
        End If

        Return color
    End Function

    Function getMonoCount() As String
        Dim mono_counter As String = getNextSNMP("1.3.6.1.4.1.11.2.3.9.4.2.1.4.1.2.6")

        Return mono_counter
    End Function

    Function getColorCount() As String
        Dim color_counter As String = getNextSNMP("1.3.6.1.4.1.11.2.3.9.4.2.1.4.1.2.7")

        Return color_counter
    End Function

    Function getSerialNumber() As String
        Dim serial_number As String = getNextSNMP("1.3.6.1.4.1.11.2.3.9.4.2.1.1.3.3")

        Return serial_number
    End Function

    Function getModelNumber() As String
        Return getNextSNMP("1.3.6.1.4.1.11.2.4.3.1.10")
    End Function


    Function getGeneric(ps As PrinterStructure) As PrinterStructure

        Dim printer_structure As PrinterStructure = ps
        Dim mono_counter As String = getMonoCount()
        Dim color_counter As String = getColorCount()

        ' Validation
        If ps.general_color = True Then
            If color_counter > 0 Then
                printer_structure.counter_color_total = getColorCount()
                If mono_counter > 0 Then
                    printer_structure.counter_mono_total = getMonoCount()
                End If
            End If
        Else
            ' Default Black Printer
            printer_structure.counter_mono_total = printer_structure.counter_total
        End If

        Return printer_structure
    End Function

End Class

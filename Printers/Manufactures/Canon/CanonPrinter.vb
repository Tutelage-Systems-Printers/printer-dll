Public Class CanonPrinter

    Public Function collect(ByVal ps As PrinterStructure) As PrinterStructure
        Debug.WriteLine("Collecting Default Canon Information")

        Dim printer_structure As New PrinterStructure

        Dim serial_number As String = getSerial()
        Dim counter_total As String = getTotalCount()
        Dim counter_mono As String = getMonoCount()
        Dim counter_color As String = getColorCount()

        printer_structure = ps

        ' Add Default Settings to Structure
        If serial_number.Trim.Length > 0 Then printer_structure.general_serial = serial_number
        If counter_total.Trim.Length > 0 Then printer_structure.counter_total = counter_total
        If counter_mono.Trim.Length > 0 Then printer_structure.counter_mono_total = counter_mono
        If counter_color.Trim.Length > 0 Then printer_structure.counter_color_total = counter_color

        ' Canon Page Counts 1.3.6.1.4.1.1602.1.11.1.3.1.4.*
        ' Black Total: 108
        ' Color Total = sum(Below)
        ' Total Full Color Large: 122
        ' Total Full Color Small: 123

        Return printer_structure
    End Function

    Function getSerial() As String
        Debug.WriteLine("Canon Serial Number")

        Dim serial_number As String = getNextSNMP("1.3.6.1.4.1.1602.1.2.1.4")

        Return serial_number
    End Function

    Function getTotalCount() As String
        Debug.WriteLine("Canon Total Count")

        Dim total_counter As String = getSNMP("1.3.6.1.4.1.1602.1.11.1.3.1.4.101")

        Return total_counter
    End Function

    Function getMonoCount() As String
        Debug.WriteLine("Canon Mono Count")

        Dim mono_counter As String = getSNMP("1.3.6.1.4.1.1602.1.11.1.3.1.4.108")

        Return mono_counter
    End Function

    Function getColorCount() As String
        Debug.WriteLine("Canon Color Count")

        Dim color_counter_large As String = getSNMP("1.3.6.1.4.1.1602.1.11.1.3.1.4.122")
        Dim color_counter_small As String = getSNMP("1.3.6.1.4.1.1602.1.11.1.3.1.4.123")
        Dim color_counter As String = Val(color_counter_large) + Val(color_counter_small)

        Return color_counter
    End Function

End Class

Module HPPhotosmart

    Public Function collect(ByVal ps As PrinterStructure) As PrinterStructure
        Dim printer_structure As New PrinterStructure

        printer_structure = ps

        ' Serial Number
        printer_structure.general_serial = getSerial()

        Return printer_structure
    End Function

    Function getSerial() As String
        Dim serialString As String = getNextSNMP("1.3.6.1.4.1.11.2.3.9.1.1.7")

        serialString = serialString.Substring(serialString.IndexOf("SN"), serialString.Length - serialString.IndexOf("SN"))
        serialString = serialString.Substring(0, serialString.IndexOf(";"))
        serialString = serialString.Replace("SN:", "")

        Return serialString
    End Function

End Module

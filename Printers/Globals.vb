Public Module Globals

    ' Variables to hold the device connection information
    Public device_ip_address As String
    Public device_snmp_password As String
    Public device_snmp_timeout As Integer
    Public device_is_valid As Boolean = False

    Public Structure PrinterStructure
        Public Property information_is_printer As Boolean
        Public Property information_ip_address As Net.IPAddress
        Public Property information_community_string As String
        Public Property information_was_scanned As Boolean
        ' Information
        Public Property information_system_identifier As String ' The Manufacture's Id (System Object Identifier)
        Public Property information_engine As String ' Whos engine is it
        ' General
        Public Property general_manufacture As String ' Who Made It
        Public Property general_model As String ' Model
        Public Property general_serial As String ' Serial Number
        Public Property general_color As Boolean ' Is it color
        Public Property general_other As String ' Not used in Scanning
        Public Property general_display As String

        ' Counters
        Public Property counter_total As String ' Total Count
        Public Property counter_mono_total As String ' Total Mono Count
        Public Property counter_color_total As String ' Total Color Count
        Public Property counter_other As String ' Not used in scanning

        ' Supplies
        Public Property supplies_black As TonerStructure ' Black Toner
        Public Property supplies_cyan As TonerStructure ' Cyan Toner
        Public Property supplies_magenta As TonerStructure ' Magenta Toner
        Public Property supplies_yellow As TonerStructure ' Yellow Toner
        Public Property supplies_maintenance_kit As TonerStructure ' Maintenance Kit
        Public Property supplies_other As TonerStructure ' Not used in scanning

        ' Debug
        Public Property debug_time_start As DateTime
        Public Property debug_time_stop As DateTime
    End Structure

    Public Structure TonerStructure
        Public Property Index As String
        Public Property Maximum_Level As String
        Public Property Current_Level As String
        Public Property Percentage As String
        Public Property Part As String
    End Structure

    Public Function getSNMP(ByVal oid As String) As String
        ' Results
        Dim value As String = ""

        ' Create the SNMP Client with the Device IP Address and Community Name
        Dim snmp As New SnmpSharpNet.SimpleSnmp(device_ip_address, 161, device_snmp_password, device_snmp_timeout, 1)

        If Not snmp.Valid Then
            ' RaiseEvent
            Return ""
        End If

        ' Connect to the device
        Dim result As Dictionary(Of SnmpSharpNet.Oid, SnmpSharpNet.AsnType)
        result = snmp.Get(SnmpSharpNet.SnmpVersion.Ver1, New String() {oid})
        If result IsNot Nothing Then
            For Each kvp In result
                value = kvp.Value.ToString
            Next
        End If

        Return value
    End Function

    Public Function getNextSNMP(ByVal oid As String) As String
        ' Results
        Dim value As String = ""
        Dim oid_results As Dictionary(Of SnmpSharpNet.Oid, SnmpSharpNet.AsnType)

        ' Setup the SNMP Request
        Dim snmp As New SnmpSharpNet.SimpleSnmp(device_ip_address, 161, device_snmp_password, device_snmp_timeout, 1)

        ' Validate IP
        If Not snmp.Valid Then
            Return value
        End If

        ' Make the request
        Try
            oid_results = snmp.GetNext(SnmpSharpNet.SnmpVersion.Ver1, New String() {oid})

            If oid_results IsNot Nothing Then
                value = oid_results.First.Value.ToString()
            End If
        Catch
        End Try


        Return value
    End Function

End Module

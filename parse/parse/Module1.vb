﻿Module Module1
    Dim verbose As Boolean = False


    Sub Main()
        Dim part As New List(Of String)
        Dim net As New List(Of String)

        Dim partref As New Dictionary(Of String, String)
        Dim partdict As New Dictionary(Of String, Dictionary(Of String, String))
        Dim all_nets As New Dictionary(Of String, Boolean)

        Console.WriteLine("Parsing netlist file ...")
        Dim parse_file As New parse_file
        parse_file.read(partref, partdict, all_nets)
        Console.WriteLine(vbTab & partdict.Count & " Gates, " & all_nets.Count & " Nets ..." & vbCrLf)
        Dim logic As New logic(partref, partdict)

        Dim inputA As String
        Dim inputB As String
        Dim inputC As String
        Dim count As Integer = 0
        While logic.compute(all_nets)
        End While

        Dim add_ok As Boolean
        add_ok = logic.check_add(all_nets, New List(Of String) From {"A0", "A1", "A2", "A3"},
                        New List(Of String) From {"B0", "B1", "B2", "B3"}, "C0",
                        New List(Of String) From {"ADD0", "ADD1", "ADD2", "ADD3"}, "C4")

        If add_ok Then
            Console.WriteLine("Adder Ok")
        Else
            Console.WriteLine("Adder fail !!! ")
        End If
        Console.WriteLine(vbCrLf)


        Dim maxprop As Integer = 0
        Console.WriteLine("Maximal propagation delay calculation ...")
        maxprop = logic.prop_delay(all_nets, New List(Of String) From {"A0", "A1", "A2", "A3", "B0", "B1", "B2", "B3", "C0"},
                                            New List(Of String) From {"ADD0", "ADD1", "ADD2", "ADD3", "C4"}, False)
        Console.WriteLine("Maximal Gate propation : " & maxprop)


        Console.Write("stop ?")
        While Console.ReadLine() <> "O"
            count = 0
            inputA = ask_bit("A")
            Console.WriteLine(inputA)
            inputB = ask_bit("B")
            Console.WriteLine(inputB)
            For i = 0 To 4
                all_nets.Item("A" & i) = Cbit(inputA(i))
                all_nets.Item("B" & i) = Cbit(inputB(i))
            Next
            inputC = ask_bit("C")
            Console.WriteLine(inputB)
            all_nets.Item("C0") = Cbit(inputC(0))

            While logic.compute(all_nets)
                count += 1
            End While


            Console.WriteLine(count & "CO : " & (all_nets.Item("C4") And &H1))
            Console.WriteLine(count & "OUT : " & (all_nets.Item("N2") And &H1) & (all_nets.Item("N24") And &H1) & (all_nets.Item("N33") And &H1) & (all_nets.Item("N38") And &H1) & (all_nets.Item("C4") And &H1))
            Console.Write(" stop ?")
        End While

        Console.ReadLine()
    End Sub

    Function Cbit(ByVal c As Char) As Boolean
        If c = "1"c Then
            Return True
        Else
            Return False
        End If
    End Function

    Function ask_bit(ByVal val As String) As String
        Console.Write(val & " : ")
        Try
            Return (Strings.StrReverse(Convert.ToString(CInt(Console.ReadLine()), 2).PadLeft(5, "0")))
        Catch
            Return ""
        End Try
    End Function

    Function cast_bit(ByVal x As Integer) As String
        Try
            Return (Strings.StrReverse(Convert.ToString(CInt(x), 2).PadLeft(5, "0")))
        Catch
            Return ""
        End Try
    End Function

    Function gen_netlist(ByVal partdict As Dictionary(Of String, Dictionary(Of String, String)), ByVal partref As Dictionary(Of String, String)) As List(Of String)
        Dim elem As New List(Of String)
        For Each p In partdict
            With p.Value
                Select Case p.Key
                    Case "74ALS00N"
                        elem.Add("7400 : " & .Item("I0") & "," & .Item("I1") & "," & .Item("O"))
                    Case "74LS151N"
                        elem.Add("74151 : " & .Item("A") & "," & .Item("B") & "," & .Item("C") & "," & .Item("G") &
                           .Item("D0") & "," & .Item("D1") & "," & .Item("D2") & "," & .Item("D3") & "," & .Item("D4") &
                           "," & .Item("D5") & "," & .Item("D6") & "," & .Item("D7") & "," &
                           .Item("W") & "," & .Item("Y"))
                End Select
            End With
        Next

        Return elem
    End Function


End Module

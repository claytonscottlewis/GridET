Public Class Help

    Private Nodes As New List(Of TreeNode)
    Private SearchString As String = ""
    Private LightGreen = Color.FromArgb(0, 167, 234, 185)

    Public Sub LoadFile(ByVal Path As String)
        Dim HelpFile = IO.File.ReadAllText(Path).Replace(vbCr, "").Replace(vbLf, " ")

        Dim Header = HelpFile.Substring(0, HelpFile.IndexOf(">", HelpFile.IndexOf("<body")) + 1)
        Dim Footer = HelpFile.Substring(HelpFile.LastIndexOf("</body"))

        Dim Pages = HelpFile.Substring(Header.Length, HelpFile.Length - Footer.Length - Header.Length).Split({"<h"}, StringSplitOptions.RemoveEmptyEntries)

        Header = Header.Replace("td p { margin-bottom: 0in;", "td p { margin-bottom: 0in; margin-top: 0in;")

        Tree.GetType.GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic Or System.Reflection.BindingFlags.Instance).SetValue(Tree, True, Nothing)
        Tree.BeginUpdate()

        Dim Levels(2) As TreeNode
        For P = 1 To Pages.Length - 1 : Dim Page = Pages(P)
            Dim Name = Page.Substring(0, Page.IndexOf("</h"))
            Name = Trim(Name.Substring(Name.LastIndexOf(">") + 1))
            Dim Html = Header & "<h" & Page & Footer

            Dim Node As TreeNode
            Dim L = Convert.ToInt32(Page(0).ToString)
            If L = 1 Then
                Node = Tree.Nodes.Add(Name, Name)
                Levels(0) = Node
            Else
                Node = Levels(L - 2).Nodes.Add(Name, Name)
                Levels(L - 1) = Node
            End If
            Node.Tag = New HelpPage(Name, Html, GetFragments(Html.ToUpper))
            Nodes.Add(Node)
        Next
        Tree.ExpandAll()
        Tree.SelectedNode = Nodes(0)
        Tree.SelectedNode.EnsureVisible()
        Tree.EndUpdate()
    End Sub

    Private Sub Tree_AfterSelect(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) Handles Tree.AfterSelect
        ShowHtml(e.Node)
    End Sub

    Private Sub ShowHtml(ByVal Node As TreeNode)
        Dim Page As HelpPage = Node.Tag
        HtmlPanel.Text = Page.HighlightSearch(SearchString)
    End Sub

    Private Sub SearchBox_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SearchBox.TextChanged
        SearchString = SearchBox.Text.ToUpper
        Tree.BeginUpdate()

        If String.IsNullOrEmpty(SearchString) Then
            For Each Node As TreeNode In Nodes
                Node.Text = Node.Name
                Node.BackColor = Color.Transparent
            Next

            SearchLabel.Text = "Search"
        Else
            Dim Total As Int32 = 0

            For Each Node As TreeNode In Nodes
                Dim Page As HelpPage = Node.Tag

                Dim Count = Page.CountSearch(SearchString)
                If Count = 0 Then
                    Node.Text = Node.Name
                    Node.BackColor = Color.Transparent
                Else
                    Node.Text = Node.Name & " (" & Count & ")"
                    Node.BackColor = LightGreen
                End If

                Total += Count
            Next

            SearchLabel.Text = "Search (" & Total & String.Format(" result{0} found)", If(Total = 1, "", "s"))
        End If

        Tree.EndUpdate()
        ShowHtml(Tree.SelectedNode)
    End Sub

    Class HelpPage

        Public Fragments As Fragment()
        Public Html As String
        Public Name As String

        Sub New(ByVal Name As String, ByVal Html As String, ByVal Fragments As Fragment())
            Me.Name = Name
            Me.Html = Html
            Me.Fragments = Fragments
        End Sub

        Function CountSearch(ByVal Find As String) As Int32
            Dim C As Int32 = 0

            For Each Fragment In Fragments
                Dim Text = Fragment.Text

                Dim I As Int32 = Text.IndexOf(Find)
                While I > -1
                    C += 1
                    I += Find.Length
                    I = Text.IndexOf(Find, I)
                End While
            Next

            Return C
        End Function

        Function HighlightSearch(ByVal Find As String) As String
            If String.IsNullOrEmpty(Find) Then
                Return Html
            Else
                Dim Indices As New List(Of Int32)
                Dim Length = Find.Length

                For Each Fragment In Fragments
                    Dim Text = Fragment.Text
                    Dim Index = Fragment.Index

                    For I = 0 To Text.Length - Length - 1
                        If Text.Substring(I, Length) = Find Then
                            Indices.Add(Index + I)
                            I += Length
                        End If
                    Next
                Next

                If Indices.Count = 0 Then
                    Return Html
                Else
                    Dim SB As New System.Text.StringBuilder

                    Dim I = 0
                    For Each Index In Indices
                        SB.Append(Html.Substring(I, Index - I))
                        SB.Append("<span style=""background-color: rgb(167, 234, 185)"">")
                        SB.Append(Html.Substring(Index, Length))
                        SB.Append("</span>")
                        I = Index + Length
                    Next
                    SB.Append(Html.Substring(I))

                    Return SB.ToString
                End If
            End If
        End Function

    End Class

    Class Fragment

        Public Index As Int32
        Public Text As String

        Sub New(ByVal Index As Int32, ByVal Text As String)
            Me.Index = Index
            Me.Text = Text
        End Sub

        Public Overrides Function ToString() As String
            Return Text
        End Function

    End Class

    Private Function GetFragments(ByVal Text As String) As Fragment()
        Dim Fragments As New List(Of Fragment)

        Dim Length = Text.Length - 3
        For T = 0 To Length
            If Text.Substring(T, 3) = "<P " Then
                Dim I As Int32 = T
                For I = T + 3 To Length
                    If Text(I) = ">" Then
                        I += 1
                        Exit For
                    End If
                Next

                For J = I To Length
                    If Text.Substring(J, 4) = "</P>" Then
                        Dim L = J - I
                        If L > 0 Then
                            Dim F = Text.Substring(I, J - I)
                            If Not String.IsNullOrWhiteSpace(F) Then Fragments.Add(New Fragment(I, F))
                        End If
                        T = J + 3
                        Exit For
                    ElseIf Text(J) = "<" Then
                        Dim L = J - I
                        If L > 0 Then
                            Dim F = Text.Substring(I, J - I)
                            If Not String.IsNullOrWhiteSpace(F) Then Fragments.Add(New Fragment(I, F))
                        End If
                        For K = J + 1 To Length
                            If Text(K) = ">" Then
                                J = K
                                I = K + 1
                                Exit For
                            End If
                        Next
                    End If
                Next
            End If
        Next

        Return Fragments.ToArray
    End Function

End Class

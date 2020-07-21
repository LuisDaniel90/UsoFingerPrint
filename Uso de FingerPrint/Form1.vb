Imports DPFP
Imports DPFP.Capture
Imports System.Text
Imports MySql.Data.MySqlClient
Imports System.IO


Public Class Form1
    Implements DPFP.Capture.EventHandler

    Private Captura As DPFP.Capture.Capture
    Private Enroller As DPFP.Processing.Enrollment
    Private Delegate Sub delegadoMuestra(ByVal text As String)
    Private Delegate Sub delegadoControles()
    Private template As DPFP.Template

    'Dim con As MySqlConnection




    Private Sub mostrarVeces(ByVal texto As String)
        If (vecesDedo.InvokeRequired) Then
            Dim deleg As New delegadoMuestra(AddressOf mostrarVeces)
            Me.Invoke(deleg, New Object() {texto})
        Else
            vecesDedo.Text = texto
        End If
    End Sub

    Private Sub modificarControles()
        If (btnGuardar.InvokeRequired) Then
            Dim deleg As New delegadoControles(AddressOf modificarControles)
            Me.Invoke(deleg, New Object() {})
        Else
            btnGuardar.Enabled = True
            txtNombre.Enabled = True

        End If
    End Sub

    Protected Overridable Sub Init()
        Try
            Captura = New Capture()
            If Not Captura Is Nothing Then
                Captura.EventHandler = Me
                Enroller = New DPFP.Processing.Enrollment()
                Dim text As New StringBuilder()
                text.AppendFormat("Necesitas pasar el dedo {0} veces", Enroller.FeaturesNeeded)
                vecesDedo.Text = text.ToString()

            Else
                MessageBox.Show("No se puedo instanciar la cpatura")
            End If
        Catch ex As Exception
            MessageBox.Show("No se pudo iniciaizar la captura")
        End Try
    End Sub
    Protected Sub iniciarCaptura()
        If Not Captura Is Nothing Then
            Try
                Captura.StartCapture()
            Catch ex As Exception
                MessageBox.Show("No se pudo iniciar captura")
            End Try
        End If
    End Sub
    Protected Sub pararCaptura()
        If Not Captura Is Nothing Then
            Try
                Captura.StopCapture()
            Catch ex As Exception
                MessageBox.Show("No se pudo detener la captura")
            End Try
        End If
    End Sub




    Public Sub OnComplete(ByVal Capture As Object, ByVal ReaderSerialNumber As String, ByVal Sample As DPFP.Sample) Implements DPFP.Capture.EventHandler.OnComplete
        ponerImagen(ConvertirSampleaMapadeBits(Sample))
        Procesar(Sample)

    End Sub

    Public Sub OnFingerGone(ByVal Capture As Object, ByVal ReaderSerialNumber As String) Implements DPFP.Capture.EventHandler.OnFingerGone

    End Sub

    Public Sub OnFingerTouch(ByVal Capture As Object, ByVal ReaderSerialNumber As String) Implements DPFP.Capture.EventHandler.OnFingerTouch
        'MessageBox.Show("Chichu si esta funcionando")
    End Sub

    Public Sub OnReaderConnect(ByVal Capture As Object, ByVal ReaderSerialNumber As String) Implements DPFP.Capture.EventHandler.OnReaderConnect

    End Sub

    Public Sub OnReaderDisconnect(ByVal Capture As Object, ByVal ReaderSerialNumber As String) Implements DPFP.Capture.EventHandler.OnReaderDisconnect

    End Sub

    Public Sub OnSampleQuality(ByVal Capture As Object, ByVal ReaderSerialNumber As String, ByVal CaptureFeedback As DPFP.Capture.CaptureFeedback) Implements DPFP.Capture.EventHandler.OnSampleQuality

    End Sub


    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Init()
        iniciarCaptura()

    End Sub

    Private Sub Form1_FormClosed(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles MyBase.FormClosed
        pararCaptura()
    End Sub
    Protected Function ConvertirSampleaMapadeBits(ByVal Sample As DPFP.Sample) As Bitmap
        Dim convertidor As New DPFP.Capture.SampleConversion() 'creanun conversor, es una variable de tipo conversor de un DPFP.sample
        Dim mapbits As Bitmap = Nothing
        convertidor.ConvertToPicture(Sample, mapbits)
        Return mapbits
    End Function

    Private Sub ponerImagen(ByVal bmp)
        ImagenHuella.Image = bmp
    End Sub

    Protected Function extraerCracteristicas(ByVal Sample As DPFP.Sample, ByVal Purpose As DPFP.Processing.DataPurpose) As DPFP.FeatureSet
        Dim extractor As New DPFP.Processing.FeatureExtraction()
        Dim alimentacion As DPFP.Capture.CaptureFeedback = DPFP.Capture.CaptureFeedback.None
        Dim caracteristicas As New DPFP.FeatureSet()
        extractor.CreateFeatureSet(Sample, Purpose, alimentacion, caracteristicas)

        If (alimentacion = DPFP.Capture.CaptureFeedback.Good) Then
            Return caracteristicas
        Else
            Return Nothing
        End If

    End Function

    Protected Sub Procesar(ByVal Sample As DPFP.Sample)
        Dim caracteristicas As DPFP.FeatureSet = extraerCracteristicas(Sample, DPFP.Processing.DataPurpose.Enrollment)
        If (Not caracteristicas Is Nothing) Then
            Try
                Enroller.AddFeatures(caracteristicas)
            Finally
                Dim text As New StringBuilder()
                text.AppendFormat("Necesitas pasar el dedo {0} veces", Enroller.FeaturesNeeded)
                mostrarVeces(text.ToString())
                Select Case Enroller.TemplateStatus
                    Case DPFP.Processing.Enrollment.Status.Ready
                        template = Enroller.Template
                        pararCaptura()
                        modificarControles()
                    Case DPFP.Processing.Enrollment.Status.Failed
                        Enroller.Clear()
                        pararCaptura()
                        iniciarCaptura()
                End Select
            End Try
        End If
    End Sub

    Private Sub btnGuardar_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnGuardar.Click

        'con = New MySqlConnection
        'Dim comando As New MySqlCommand
        'Dim cad As String
        'cad = "server=localhost; user id=root; database=fingerprint"
        'con.ConnectionString = cad
        'Dim sql As String

        'Using fm As New MemoryStream(template.Bytes)
        '    comando.Parameters.AddWithValue("huella", fm.ToArray())
        '    var = fm.ToArray()

        'End Using

        'template = Enroller.Template

        'If (txtNombre.Text.ToString().Equals("")) Then
        '    MessageBox.Show("debes de llenar el campo nombre")
        'Else
        '    sql = "insert into usuarios(Nombre,huella)values"

        '    sql = sql + "('" + txtNombre.Text.ToString() + "','" + var + "')"

        '    Dim var As Long




        '    comando.Connection = con
        '    comando.CommandType = CommandType.Text
        '    comando.CommandText = sql
        '    con.Open()
        '    comando.ExecuteNonQuery()
        '    con.Close()
        '    con.Dispose()



        Dim builderconex As New MySqlConnectionStringBuilder()
        builderconex.Server = "localhost"
        builderconex.UserID = "root"
        builderconex.Password = ""
        builderconex.Database = "fingerprint"
        Dim conexion As New MySqlConnection(builderconex.ToString())
        conexion.Open()
        Dim cmd As New MySqlCommand()
        cmd = conexion.CreateCommand

        If (txtNombre.Text.ToString().Equals("")) Then
            MessageBox.Show("debes de llenar el campo nombre")
        Else
            cmd.CommandText = "INSERT INTO Usuarios(Nombre,huella) VALUES(?,?)"
            'cmd.CommandText = "INSERT INTO Usuarios(Nombre,huella) VALUES('" + txtNombre.Text.ToString() + "','" + fm + "',?)"
            cmd.Parameters.AddWithValue("Nombre", txtNombre.Text.ToString())

            Using fm As New MemoryStream(template.Bytes)
                cmd.Parameters.AddWithValue("huella", fm.ToArray()) ' cargamos la huella
            End Using

            cmd.ExecuteNonQuery()
            cmd.Dispose()
            conexion.Close()
            conexion.Dispose()
            MessageBox.Show("OK")
            btnGuardar.Enabled = False
            txtNombre.Enabled = False

        End If
    End Sub
End Class

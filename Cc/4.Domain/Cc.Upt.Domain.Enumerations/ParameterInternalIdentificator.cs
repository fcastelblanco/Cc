using System.ComponentModel;

namespace Cc.Upt.Domain.Enumerations
{
    public enum ParameterInternalIdentificator
    {
        [Description("Servidor smtp")]
        SmtpServer,
        [Description("Usuario servidor smtp")]
        UserSmtpServer,
        [Description("E-mail envío correos")]
        EmailSender,
        [Description("E-mail receptor correos (Interno)")]
        EmailReceiver,
        [Description("Ssl habilitado")]
        EnableSsl,
        [Description("Puerto smtp")]
        SendingPort,

        [Description("Clave e-mail envío correos")]
        PasswordSender,
        [Description("Servidor ftp")]
        FtpPath,
        [Description("Usuario ftp")]
        FtpUser,
        [Description("Clave usuario ftp")]
        FtpPassword,
        [Description("Tamaño del buffer")]
        BufferSize,

        [Description("Días de validez del token")]
        ValidDaysToken,
        [Description("Versión del software")]
        VersionValue,
        [Description("Longitud mínima de notas de versión")]
        NotesVersionMinLength,

        [Description("Días de expiración de releases")]
        ReleaseDaysExpiration,
        [Description("Intervalo de ejecución de procesos internos (en minutos)")]
        IntervalExecutionInternalProcess,
        [Description("Ruta local para posterior descarga de archivos")]
        LocalPathForDownloadFiles
    }
}
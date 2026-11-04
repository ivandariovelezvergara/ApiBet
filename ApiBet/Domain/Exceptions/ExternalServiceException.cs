namespace Domain.Exceptions
{
    /// <summary>
    /// Error producido al comunicarse con un servicio externo.
    /// </summary>
    public sealed class ExternalServiceException : Exception
    {
        /// <summary>
        /// Inicializa la excepción con un mensaje descriptivo.
        /// </summary>
        /// <param name="message">Detalle del fallo en el servicio externo.</param>
        public ExternalServiceException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Inicializa la excepción con un mensaje y la causa original.
        /// </summary>
        /// <param name="message">Detalle del fallo en el servicio externo.</param>
        /// <param name="innerException">Excepción original capturada.</param>
        public ExternalServiceException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

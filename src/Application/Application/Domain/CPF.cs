using System;

namespace Concepta.Application.Domain
{
    /// <summary>
    /// Corresponde à um identificador único de um membro
    /// </summary>
    public class CPF
    {
        private string Value;

        public CPF(string cpf) => Value = cpf.ValidarCPF() ? cpf : throw new InvalidCastException("Invalid CPF");

        public CPF(Int64 cpf)
        {
            //HIGH: reabiliatar a validação
            Value = cpf.ToString(); //.ValidarCPF() ? cpf.ToString() : throw new InvalidCastException("Invalid CPF");
        }

        public static implicit operator Int64(CPF id) => Convert.ToInt64(id.Value.Clear());
        public static implicit operator string(CPF id) => id.Value;

        public static implicit operator CPF(string id) => new CPF(id);
        public static implicit operator CPF(Int64 id) => new CPF(id);
    }

    internal static class CPFExtensions
    {
        public static bool ValidarCPF(this string cpf)
        {
            var cpfFormatado = cpf.FormatarCPF();
            if (string.IsNullOrEmpty(cpfFormatado))
                return false;

            var multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            var multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            string tempCpf, digito;
            int soma, resto;

            cpfFormatado = cpfFormatado.Trim();
            cpfFormatado = cpfFormatado.Replace(".", "").Replace("-", "");
            if (cpfFormatado.Length != 11)
                return false;

            switch (cpfFormatado)
            {
                case "00000000000":
                    return false;
                case "11111111111":
                    return false;
                case "2222222222":
                    return false;
                case "33333333333":
                    return false;
                case "44444444444":
                    return false;
                case "55555555555":
                    return false;
                case "66666666666":
                    return false;
                case "77777777777":
                    return false;
                case "88888888888":
                    return false;
                case "99999999999":
                    return false;
            }

            tempCpf = cpfFormatado.Substring(0, 9);
            soma = 0;
            for (int i = 0; i < 9; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito = resto.ToString();
            tempCpf = tempCpf + digito;
            soma = 0;

            for (int i = 0; i < 10; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito = digito + resto.ToString();

            return cpfFormatado.EndsWith(digito);
        }


        /// <summary>
        /// Formata cpf
        /// </summary>
        /// <param name="texto">cpf</param>
        /// <returns>Cpf Formatado</returns>
        public static string FormatarCPF(this string texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return string.Empty;

            if (ulong.TryParse(texto.Clear(), out var value))
                return value.ToString(@"000\.000\.000\-00");
            else
                return string.Empty;
        }

        /// <summary>
        /// Remove caracteres inválidos
        /// </summary>
        /// <param name="text">texto</param>
        /// <returns>Texto sem caracteres inválidos</returns>
        public static string Clear(this string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;
            return text.Replace(".", string.Empty)
                       .Replace("-", string.Empty)
                       .Replace("/", string.Empty)
                       .Replace("  ", " ");
        }

    }
}

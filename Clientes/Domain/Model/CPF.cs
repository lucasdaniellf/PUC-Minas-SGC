using System;
using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace Clientes.Domain.Model
{
    public class CPF
    {
        public string Numero { get; private set; } = string.Empty;

        public CPF(string cpf)
        {
            if (ValidarCpf(cpf))
            {
                Numero = cpf;
            }
        }

        private bool ValidarCpf(string cpf)
        {

            if (string.IsNullOrEmpty(cpf))
            {
                throw new ClienteException("CPF não deve ser vazio");
            }

            if (cpf.Length > 11)
            {
                throw new ClienteException("CPF não deve ter mais de 11 dígitos");
            }

            while (cpf.Length < 11)
            {
                cpf = string.Concat('0', cpf);
            }

            int[] digitos = new int[11];

            if (!ValidarNumerosCpf(cpf, ref digitos))
            {
                throw new ClienteException("CPF inválido, verifique se o digitou corretamente");
            }

            if (!ValidarPrimeiroDigitoVerificador(digitos) || !ValidarSegundoDigitoVerificador(digitos))
            {
                throw new ClienteException("CPF inválido, verifique se o digitou corretamente");
            }

            return true;
        }

        private bool ValidarNumerosCpf(string cpf, ref int[] digitos)
        {
            if (digitos.Length != 11)
            {
                return false;
            }

            for (var i = 0; i < 11; i++)
            {
                string digitoStr = char.ToString(cpf[i]);
                if (!int.TryParse(digitoStr, out int digito))
                {
                    return false;
                }

                digitos[i] = digito;
            }

            return true;
        }

        private bool ValidarPrimeiroDigitoVerificador(int[] digitos)
        {
            int somaPrimeiroDigito = 0;
            for (var n = 0; n <= 8; n++)
            {
                somaPrimeiroDigito += digitos[n] * (10 - n);
            }

            if (somaPrimeiroDigito % 11 < 2)
            {
                if (digitos[9] != 0)
                    return false;
            }
            else
            {
                if (digitos[9] != 11 - somaPrimeiroDigito % 11)
                    return false;
            }
            return true;
        }

        private bool ValidarSegundoDigitoVerificador(int[] digitos)
        {
            int somaSegundoDigito = 0;
            for (var n = 0; n <= 9; n++)
            {
                somaSegundoDigito += digitos[n] * (11 - n);
            }

            if (somaSegundoDigito % 11 < 2)
            {
                if (digitos[10] != 0)
                    return false;
            }
            else
            {
                if (digitos[10] != 11 - somaSegundoDigito % 11)
                    return false;
            }
            return true;
        }
    }
}
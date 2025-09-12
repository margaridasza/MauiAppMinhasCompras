﻿using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiAppMinhasCompras.Models
{
    public class Produto
    {
        string _descricao;

        [PrimaryKey, AutoIncrement] // Define que 'Id' é a chave primária no banco e será incrementada automaticamente
        public int Id { get; set; }
        public string Descricao // Propriedade do produto (valida se for nula)
        {
            get => _descricao; // Retorna o valor da descrição
            set
            {
                if (value == null)
                {
                    throw new Exception("Por favor, preencha a descrição");
                }

                _descricao = value;
            }
        }
        public double Quantidade { get; set; }
        public double Preco { get; set; }
        public string Categoria { get; set; }
        public double Total { get => Quantidade * Preco; } // Calcula total automaticamente
    }
}
using System;
using System.Collections.Generic;

namespace Massena.Infrastructure.Core.Infrastructure
{
    public interface IRepository : IDisposable
    {
        /// <summary>
        /// Salva as alterações da base
        /// </summary>
        /// <returns></returns>
        int SaveChanges();
    }


    public interface IRepository<TEntity> : IRepository where TEntity : class
    {
        /// <summary>
        /// Adiciona um novo registro
        /// </summary>
        /// <param name="domain"></param>
        void Add(TEntity domain);

        /// <summary>
        /// Adiciona uma lista de registros
        /// </summary>
        /// <param name="domains"></param>
        void Add(IEnumerable<TEntity> domains);

        /// <summary>
        /// Obtem um registro por Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        TEntity GetById(int id);

        /// <summary>
        /// Obtem todos os registros da tabela. USE COM CUIDADO!
        /// </summary>
        /// <returns></returns>
        IEnumerable<TEntity> GetAll();

        /// <summary>
        /// Atualiza um registro no banco de dados
        /// </summary>
        /// <param name="domain"></param>
        void Update(TEntity domain);

        /// <summary>
        /// Remove um registro pelo id do banco de dados!
        /// </summary>
        /// <param name="id"></param>
        void Remove(int id);
    }
}

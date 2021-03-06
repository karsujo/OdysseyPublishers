using Application.Common;
using AutoMapper;
using Dapper;
using Infrastructure.Authors;
using Infrastructure.Books;
using Microsoft.Extensions.Options;
using OdysseyPublishers.Domain;
using OdysseyPublishers.Infrastructure.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Xunit;

namespace Infrastructure.Tests
{
    public class RepositoryBaseTests
    {

        private readonly SqlRepositoryBase _repo;

        public RepositoryBaseTests()
        {
            var config = new MapperConfiguration(opt =>
            {
                opt.AddProfile(new AuthorDbProfile());
                opt.AddProfile(new BookDbProfile());
            });

            var mapper = config.CreateMapper();
            var opt = Options.Create(new PersistenceConfigurations());
            opt.Value.ConnectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Odyssey;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            _repo = new SqlRepositoryBase(opt);
        }
        [Fact]
        public void DbConnectionTest()
        {
            Assert.IsType<SqlConnection>(_repo.GetDbConnection());
        }

        [Fact]
        public void QueryDatabaseGenericTest()
        {
            string sql = @"SELECT 
              au_id AuthorId,
              au_fname FirstName, 
              au_lname LastName,
              phone,
              address,
              city,
              state,
              zip
            FROM
              AUTHORS ";
            var result = _repo.QueryDatabase<Author>(sql, null);
            Assert.IsType<List<Author>>(result);
            Assert.NotEmpty(result);
        }

        [Fact]

        public void QueryDatabaseTest()
        {
            string sql = @"SELECT 
              au_id AuthorId,
              au_fname FirstName, 
              au_lname LastName,
              phone,
              address,
              city,
              state,
              zip
            FROM
              AUTHORS ";

            var result = _repo.QueryDatabase(sql).ToList();
            Assert.NotEmpty(result);

        }

        [Fact]
        public void QueryDatabaseSingleGenericTest()
        {
            string sql = "select* from authors where au_id = @AuthorId";
            var parameters = new DynamicParameters();
            parameters.Add("@AuthorId",getSampleAuthor().AuthorId,DbType.String, ParameterDirection.Input);
            var result = _repo.QueryDatabaseSingle<Author>(sql, parameters);
            Assert.IsType<Author>(result);
        }

        private Author getSampleAuthor()
        {
            string sql = @"SELECT 
              au_id AuthorId,
              au_fname FirstName, 
              au_lname LastName,
              phone,
              address,
              city,
              state,
              zip
            FROM
              AUTHORS ";

            return  _repo.QueryDatabase<Author>(sql,null).ToList().First();
        }
    }
}

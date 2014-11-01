﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simple.Data.Ado;
using Simple.Data.Mocking;

namespace Simple.Data.IntegrationTest.Query
{
    using Mocking.Ado;
    using NUnit.Framework;
    using Xunit;

    public class WithCountTest : DatabaseIntegrationContext
    {
        protected override void SetSchema(MockSchemaProvider schemaProvider)
        {
            schemaProvider.SetTables(new[] { "dbo", "Users", "BASE TABLE" });

            schemaProvider.SetColumns(new[] { "dbo", "Users", "Name" },
                                      new[] { "dbo", "Users", "Password" });
        }

        //[Fact]
// ReSharper disable InconsistentNaming
        public async void WithTotalCountShouldCreateCompoundQuery_ObsoleteFutureVersion()
// ReSharper restore InconsistentNaming
        {
            const string expected = @"select count(*) from [dbo].[users] where [dbo].[users].[name] = @p1_c0; " +
                @"select [dbo].[users].[name],[dbo].[users].[password] from [dbo].[users] where [dbo].[users].[name] = @p1_c1";

            Future<int> count;
            var q = TargetDb.Users.QueryByName("Foo")
                .WithTotalCount(out count);

            EatException<InvalidOperationException>(() => q.ToList());

            GeneratedSqlIs(expected);
        }

        //[Fact]
        public async void WithTotalCountShouldCreateCompoundQuery()
        {
            const string expected = @"select count(*) from [dbo].[users] where [dbo].[users].[name] = @p1_c0; " +
                @"select [dbo].[users].[name],[dbo].[users].[password] from [dbo].[users] where [dbo].[users].[name] = @p1_c1";

            Promise<int> count;
            var q = TargetDb.Users.QueryByName("Foo")
                .WithTotalCount(out count);

            await EatExceptionAsync<InvalidOperationException>(() => q.ToList());

            GeneratedSqlIs(expected);
        }

        //[Fact]
        public async void WithTotalCountWithExplicitSelectShouldCreateCompoundQuery()
        {
            const string expected = @"select count(*) from [dbo].[users] where [dbo].[users].[name] = @p1_c0; " +
                @"select [dbo].[users].[name] from [dbo].[users] where [dbo].[users].[name] = @p1_c1";

            Promise<int> count;
            var q = TargetDb.Users.QueryByName("Foo")
                .Select(TargetDb.Users.Name)
                .WithTotalCount(out count);

            EatException<InvalidOperationException>(() => q.ToList());

            GeneratedSqlIs(expected);
        }
    }
}
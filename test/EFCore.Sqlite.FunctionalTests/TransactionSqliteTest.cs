// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.EntityFrameworkCore;

#nullable disable

public class TransactionSqliteTest(TransactionSqliteTest.TransactionSqliteFixture fixture) : TransactionTestBase<TransactionSqliteTest.TransactionSqliteFixture>(fixture)
{
    protected override bool SnapshotSupported
        => false;

    protected override DbContext CreateContextWithConnectionString()
    {
        var options = Fixture.AddOptions(
                new DbContextOptionsBuilder().UseSqlite(TestStore.ConnectionString)
                    .ConfigureWarnings(w => w.Log(RelationalEventId.AmbientTransactionWarning)))
            .UseInternalServiceProvider(Fixture.ServiceProvider);

        return new DbContext(options.Options);
    }

    public class TransactionSqliteFixture : TransactionFixtureBase
    {
        protected override ITestStoreFactory TestStoreFactory
            => SharedCacheSqliteTestStoreFactory.Instance;

        public override void Reseed()
        {
            using var context = CreateContext();
            context.Set<TransactionCustomer>().RemoveRange(context.Set<TransactionCustomer>());
            context.Set<TransactionOrder>().RemoveRange(context.Set<TransactionOrder>());
            context.SaveChanges();

            Seed(context);
        }

        public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
            => base.AddOptions(builder)
                .ConfigureWarnings(w => w.Log(RelationalEventId.AmbientTransactionWarning));
    }
}

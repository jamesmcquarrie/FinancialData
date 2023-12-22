using FinancialData.Common.Configuration;
using FinancialData.Common.Dtos;
using FinancialData.Domain.Entities;
using FinancialData.Domain.Enums;
using System.Globalization;
using AutoFixture;
using Ardalis.SmartEnum.AutoFixture;

namespace FinancialData.Common.Customizations;

public class MetadataDtoCustomization : ICustomization
{
    private readonly TimeSeriesArguments _args;

    public MetadataDtoCustomization(TimeSeriesArguments args)
    {
        _args = args;
    }

    public void Customize(IFixture fixture)
    {
        fixture.Customize<MetadataDto>(composer => composer
            .With(m => m.Symbol, _args.Symbol)
            .With(m => m.Interval, _args.Interval));           
    }
}

public class TimeSeriesDtoCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customizations.Add(new RandomDateTimeSequenceGenerator());

        fixture.Customize<TimeSeriesDto>(composer => composer
            .With(x => x.Datetime, () => fixture.Create<DateTime>().ToString(CultureInfo.InvariantCulture))
            .With(x => x.High, () => fixture.Create<double>().ToString(CultureInfo.InvariantCulture))
            .With(x => x.Low, () => fixture.Create<double>().ToString(CultureInfo.InvariantCulture))
            .With(x => x.Open, () => fixture.Create<double>().ToString(CultureInfo.InvariantCulture))
            .With(x => x.Close, () => fixture.Create<double>().ToString(CultureInfo.InvariantCulture))
            .With(x => x.Volume, () => fixture.Create<int>().ToString(CultureInfo.InvariantCulture)));
    }
}

public class MetadataCustomization : ICustomization
{
    private readonly TimeSeriesArguments _args;

    public MetadataCustomization(TimeSeriesArguments args)
    {
        _args = args;
    }

    public void Customize(IFixture fixture)
    {
        fixture.Customize<Metadata>(composer => composer
            .With(m => m.Symbol, _args.Symbol)
            .With(m => m.Interval, _args.Interval)
            .Without(x => x.Stock));
    }
}

public class TimeSeriesCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customizations.Add(new RandomDateTimeSequenceGenerator());

        fixture.Customize<TimeSeries>(composer => composer
            .With(x => x.Datetime, () => fixture.Create<DateTime>())
            .With(x => x.High, () => fixture.Create<double>())
            .With(x => x.Low, () => fixture.Create<double>())
            .With(x => x.Open, () => fixture.Create<double>())
            .With(x => x.Close, () => fixture.Create<double>())
            .With(x => x.Volume, () => fixture.Create<int>())
            .Without(x => x.Stock));
    }
}

public class TimeSeriesArgumentsCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customize<TimeSeriesArguments>(composer => composer
            .With(x => x.Symbol, () => fixture.Create<string>())
            .With(x => x.Interval, () => fixture.Customize(new SmartEnumCustomization())
                    .Create<Interval>()
                    .Name)
            .With(x => x.OutputSize, () => fixture.Create<int>()));
    }
}

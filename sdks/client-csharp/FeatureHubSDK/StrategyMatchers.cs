

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using IO.FeatureHub.SSE.Model;
using NodaTime;

public interface IPercentageCalculator
{
  int DetermineClientPercentage(string userKey, string id);
}

public class PercentageMumur3Calculator : IPercentageCalculator
{
  public static int MAX_PERCENTAGE = 1000000;

  public int DetermineClientPercentage(string UserKey, string Id)
  {
    throw new System.NotImplementedException();
  }
}

public interface IStrategyMatcher
{
  bool Match(string suppliedValue, RolloutStrategyAttribute attr);
}

public interface IMatcherRepository
{
  IStrategyMatcher FindMatcher(RolloutStrategyAttribute attr);
}

public class MatcherRegistry : IMatcherRepository
{
  public IStrategyMatcher FindMatcher(RolloutStrategyAttribute attr)
  {
    switch (attr.Type)
    {
      case RolloutStrategyFieldType.STRING:
        return new StringMatcher();
      case RolloutStrategyFieldType.SEMANTICVERSION:
        break;
      case RolloutStrategyFieldType.NUMBER:
        return new NumberMatcher();
      case RolloutStrategyFieldType.DATE:
        return new DateMatcher();
      case RolloutStrategyFieldType.DATETIME:
        return new DateTimeMatcher();
      case RolloutStrategyFieldType.BOOLEAN:
        return new BooleanMatcher();
      case RolloutStrategyFieldType.IPADDRESS:
        break;
      case null:
        return new FallthroughMatcher();
      default:
        return new FallthroughMatcher();
    }

    return new FallthroughMatcher();
  }

  private class FallthroughMatcher : IStrategyMatcher
  {
    public bool Match(string suppliedValue, RolloutStrategyAttribute attr)
    {
      return false;
    }
  }
}

internal class BooleanMatcher : IStrategyMatcher
{
  public bool Match(string suppliedValue, RolloutStrategyAttribute attr)
  {
    var val = "true".Equals(suppliedValue);

    if (attr.Conditional == RolloutStrategyAttributeConditional.EQUALS)
    {
      return val == (bool)attr.Values[0];
    }

    if (attr.Conditional == RolloutStrategyAttributeConditional.NOTEQUALS)
    {
      return val != (bool)attr.Values[0];
    }

    return false;
  }
}

internal class StringMatcher : IStrategyMatcher
{
  public bool Match(string suppliedValue, RolloutStrategyAttribute attr)
  {
    var vals = attr.Values.Where(v => v != null).Select(v => v.ToString()).ToList();

    switch (attr.Conditional)
    {
      case RolloutStrategyAttributeConditional.EQUALS:
        return vals.Any(v => v.Equals(suppliedValue));
      case RolloutStrategyAttributeConditional.ENDSWITH:
        return vals.Any(suppliedValue.EndsWith);
      case RolloutStrategyAttributeConditional.STARTSWITH:
        return vals.Any(suppliedValue.StartsWith);
      case RolloutStrategyAttributeConditional.GREATER:
        return vals.Any(v => string.Compare(suppliedValue, v, StringComparison.Ordinal) > 0);
      case RolloutStrategyAttributeConditional.GREATEREQUALS:
        return vals.Any(v => string.Compare(suppliedValue, v, StringComparison.Ordinal) >= 0);
      case RolloutStrategyAttributeConditional.LESS:
        return vals.Any(v => string.Compare(suppliedValue, v, StringComparison.Ordinal) < 0);
      case RolloutStrategyAttributeConditional.LESSEQUALS:
        return vals.Any(v => string.Compare(suppliedValue, v, StringComparison.Ordinal) >= 0);
      case RolloutStrategyAttributeConditional.NOTEQUALS:
        return !vals.Any(v => v.Equals(suppliedValue));
      case RolloutStrategyAttributeConditional.INCLUDES:
        return vals.Any(suppliedValue.Contains);
      case RolloutStrategyAttributeConditional.EXCLUDES:
        return !vals.Any(suppliedValue.Contains);
      case RolloutStrategyAttributeConditional.REGEX:
        return vals.Any(v => Regex.IsMatch(suppliedValue, v));
      case null:
        return false;
      default:
        return false;
    }
  }
}

internal class DateMatcher : IStrategyMatcher
{
  public bool Match(string suppliedValue, RolloutStrategyAttribute attr)
  {
      var suppliedDate = LocalDate.FromDateTime(DateTime.Parse(suppliedValue));
      var vals = attr.Values.Where(v => v != null).Select(v => v.ToString()).ToList();

      switch (attr.Conditional)
      {
        case RolloutStrategyAttributeConditional.EQUALS:
        case RolloutStrategyAttributeConditional.INCLUDES:
          return vals.Any(v => suppliedDate.Equals(LocalDate.FromDateTime(DateTime.Parse(v))));
        case RolloutStrategyAttributeConditional.ENDSWITH:
          return vals.Any(suppliedValue.EndsWith);
        case RolloutStrategyAttributeConditional.STARTSWITH:
          return vals.Any(suppliedValue.StartsWith);
        case RolloutStrategyAttributeConditional.GREATER:
          return vals.Any(v => suppliedDate.CompareTo(LocalDate.FromDateTime(DateTime.Parse(v))) > 0);
        case RolloutStrategyAttributeConditional.GREATEREQUALS:
          return vals.Any(v => suppliedDate.CompareTo(LocalDate.FromDateTime(DateTime.Parse(v))) >= 0);
        case RolloutStrategyAttributeConditional.LESS:
          return vals.Any(v => suppliedDate.CompareTo(LocalDate.FromDateTime(DateTime.Parse(v))) < 0);
        case RolloutStrategyAttributeConditional.LESSEQUALS:
          return vals.Any(v => suppliedDate.CompareTo(LocalDate.FromDateTime(DateTime.Parse(v))) <= 0);
        case RolloutStrategyAttributeConditional.EXCLUDES:
        case RolloutStrategyAttributeConditional.NOTEQUALS:
          return !vals.Any(v => suppliedDate.Equals(LocalDate.FromDateTime(DateTime.Parse(v.ToString()))));
        case RolloutStrategyAttributeConditional.REGEX:
          return vals.Any(v => Regex.IsMatch(suppliedValue, v));
        case null:
          return false;
        default:
          return false;
      }
  }
}

internal class DateTimeMatcher : IStrategyMatcher
{
  public bool Match(string suppliedValue, RolloutStrategyAttribute attr)
  {
    var suppliedDate = LocalDateTime.FromDateTime(DateTime.Parse(suppliedValue));
    var vals = attr.Values.Where(v => v != null).Select(v => v.ToString()).ToList();

    switch (attr.Conditional)
    {
      case RolloutStrategyAttributeConditional.EQUALS:
      case RolloutStrategyAttributeConditional.INCLUDES:
        return vals.Any(v => suppliedDate.Equals(LocalDateTime.FromDateTime(DateTime.Parse(v))));
      case RolloutStrategyAttributeConditional.ENDSWITH:
        return vals.Any(suppliedValue.EndsWith);
      case RolloutStrategyAttributeConditional.STARTSWITH:
        return vals.Any(suppliedValue.StartsWith);
      case RolloutStrategyAttributeConditional.GREATER:
        return vals.Any(v => suppliedDate.CompareTo(LocalDateTime.FromDateTime(DateTime.Parse(v))) > 0);
      case RolloutStrategyAttributeConditional.GREATEREQUALS:
        return vals.Any(v => suppliedDate.CompareTo(LocalDateTime.FromDateTime(DateTime.Parse(v))) >= 0);
      case RolloutStrategyAttributeConditional.LESS:
        return vals.Any(v => suppliedDate.CompareTo(LocalDateTime.FromDateTime(DateTime.Parse(v))) < 0);
      case RolloutStrategyAttributeConditional.LESSEQUALS:
        return vals.Any(v => suppliedDate.CompareTo(LocalDateTime.FromDateTime(DateTime.Parse(v))) <= 0);
      case RolloutStrategyAttributeConditional.EXCLUDES:
      case RolloutStrategyAttributeConditional.NOTEQUALS:
        return !vals.Any(v => suppliedDate.Equals(LocalDateTime.FromDateTime(DateTime.Parse(v.ToString()))));
      case RolloutStrategyAttributeConditional.REGEX:
        return vals.Any(v => Regex.IsMatch(suppliedValue, v));
      case null:
        return false;
      default:
        return false;
    }
  }
}

internal class NumberMatcher : IStrategyMatcher
{
  private RolloutStrategyAttribute _attr;

  private IEnumerable<decimal> DVals => _attr.Values.Where(v => v != null).Select(v => decimal.Parse(v.ToString())).ToList();

  public bool Match(string suppliedValue, RolloutStrategyAttribute attr)
  {
    this._attr = attr;
    var dec = decimal.Parse(suppliedValue);

    switch (attr.Conditional)
    {
      case RolloutStrategyAttributeConditional.EQUALS:
      case RolloutStrategyAttributeConditional.INCLUDES:
        return DVals.Any(v => v.Equals(dec));
      case RolloutStrategyAttributeConditional.ENDSWITH:
        return attr.Values.Where(v => v != null).Any(v => suppliedValue.EndsWith(v.ToString()));
      case RolloutStrategyAttributeConditional.STARTSWITH:
        return attr.Values.Where(v => v != null).Any(v => suppliedValue.StartsWith(v.ToString()));
      case RolloutStrategyAttributeConditional.GREATER:
        return DVals.Any(v => v.CompareTo(dec) > 0);
      case RolloutStrategyAttributeConditional.GREATEREQUALS:
        return DVals.Any(v => v.CompareTo(dec) >= 0);
      case RolloutStrategyAttributeConditional.LESS:
        return DVals.Any(v => v.CompareTo(dec) < 0);
      case RolloutStrategyAttributeConditional.LESSEQUALS:
        return DVals.Any(v => v.CompareTo(dec) <= 0);
      case RolloutStrategyAttributeConditional.NOTEQUALS:
      case RolloutStrategyAttributeConditional.EXCLUDES:
        return !DVals.Any(v => v.Equals(dec));
      case RolloutStrategyAttributeConditional.REGEX:
        break;
      case null:
        return false;
      default:
        return false;
    }

    return false;
  }
}


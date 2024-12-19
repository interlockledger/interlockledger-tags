using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NUnit.Framework;

namespace InterlockLedger.Tags;

[TestFixture]
public class AppPermissionsTests
{
    [Test]
    public void Constructor_ShouldInitializeProperties()
    {
        // Arrange
        ulong appId = 123;
        ulong[] actionIds = { 1, 2, 3 };

        // Act
        var permissions = new AppPermissions(appId, actionIds);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(permissions.AppId, Is.EqualTo(appId));
            Assert.That(permissions.ActionIds, Is.EquivalentTo(actionIds));
            Assert.That(permissions.TextualRepresentation, Is.EqualTo($"#{appId},{string.Join(",", actionIds)}"));
        });
    }

    [Test]
    public void InvalidBy_ShouldReturnInvalidAppPermissions()
    {
        // Arrange
        string cause = "Invalid cause";

        // Act
        var permissions = AppPermissions.InvalidBy(cause);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(permissions.AppId, Is.EqualTo(ulong.MaxValue));
            Assert.That(permissions.ActionIds, Is.Empty);
            Assert.That(permissions.InvalidityCause, Is.EqualTo(cause));
            Assert.That(permissions.TextualRepresentation, Is.EqualTo($"#?{cause}"));
        });
    }

    [Test]
    public void CanAct_ShouldReturnTrue_WhenAppIdAndActionIdMatch()
    {
        // Arrange
        ulong appId = 123;
        ulong actionId = 1;
        var permissions = new AppPermissions(appId, actionId);

        // Act
        var result = permissions.CanAct(appId, actionId);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void CanAct_ShouldReturnFalse_WhenAppIdDoesNotMatch()
    {
        // Arrange
        ulong appId = 123;
        ulong actionId = 1;
        var permissions = new AppPermissions(appId, actionId);

        // Act
        var result = permissions.CanAct(456, actionId);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void CanAct_ShouldReturnFalse_WhenActionIdDoesNotMatch()
    {
        // Arrange
        ulong appId = 123;
        ulong actionId = 1;
        var permissions = new AppPermissions(appId, actionId);

        // Act
        var result = permissions.CanAct(appId, 2);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void ToEnumerable_ShouldReturnSingleElementEnumerable()
    {
        // Arrange
        var permissions = new AppPermissions(123, 1, 2, 3);

        // Act
        var result = permissions.ToEnumerable();

        // Assert
        Assert.That(result, Is.EquivalentTo(new[] { permissions }));
    }

    [Test]
    public void AsTag_ShouldReturnTagWithAppPermissions()
    {
        // Arrange
        var permissions = new AppPermissions(123, 1, 2, 3);

        // Act
        var tag = permissions.AsTag;

        // Assert
        Assert.That(tag.Value, Is.EqualTo(permissions));
    }

    [Test]
    public void VerboseRepresentation_ShouldReturnCorrectString()
    {
        // Arrange
        var permissions = new AppPermissions(123, 1, 2, 3);

        // Act
        var result = permissions.VerboseRepresentation;

        // Assert
        Assert.That(result, Is.EqualTo("App #123 Actions 1,2,3"));
    }

    [Test]
    public void IsEmpty_ShouldReturnTrue_WhenAppIdAndActionIdsAreZero()
    {
        // Arrange
        var permissions = new AppPermissions(0);

        // Act
        var result = permissions.IsEmpty;

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void IsEmpty_ShouldReturnFalse_WhenAppIdIsNotZero()
    {
        // Arrange
        var permissions = new AppPermissions(123);

        // Act
        var result = permissions.IsEmpty;

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void Parse_ShouldReturnAppPermissions_WhenStringIsValid()
    {
        // Arrange
        string s = "#123,1,2,3";

        // Act
        var result = AppPermissions.Parse(s, null);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.AppId, Is.EqualTo(123));
            Assert.That(result.ActionIds, Is.EquivalentTo(new[] { 1, 2, 3 }));
        });
    }

    [Test]
    public void Parse_ShouldReturnInvalidAppPermissions_WhenStringIsInvalid()
    {
        // Arrange
        string s = "invalid";

        // Act
        var result = AppPermissions.Parse(s, null);

        // Assert
        Assert.That(result.IsInvalid(), Is.True);
    }

    [Test]
    public void TryParse_ShouldReturnTrue_WhenStringIsValid()
    {
        // Arrange
        string s = "#123,1,2,3";

        // Act
        var success = AppPermissions.TryParse(s, null, out var result);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(success, Is.True);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.AppId, Is.EqualTo(123));
            Assert.That(result.ActionIds, Is.EquivalentTo(new[] { 1, 2, 3 }));
        });
    }

    [Test]
    public void TryParse_ShouldReturnFalse_WhenStringIsInvalid()
    {
        // Arrange
        string s = "invalid";

        // Act
        var success = AppPermissions.TryParse(s, null, out var result);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(success, Is.False);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.IsInvalid(), Is.True);
        });
    }

    [Test]
    public void Equals_ShouldReturnTrue_WhenAppPermissionsAreEqual()
    {
        // Arrange
        var permissions1 = new AppPermissions(123, 1, 2, 3);
        var permissions2 = new AppPermissions(123, 1, 2, 3);

        // Act
        var result = permissions1.Equals(permissions2);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void Equals_ShouldReturnFalse_WhenAppPermissionsAreNotEqual()
    {
        // Arrange
        var permissions1 = new AppPermissions(123, 1, 2, 3);
        var permissions2 = new AppPermissions(456, 4, 5, 6);

        // Act
        var result = permissions1.Equals(permissions2);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void GetHashCode_ShouldReturnSameHashCode_ForEqualAppPermissions()
    {
        // Arrange
        var permissions1 = new AppPermissions(123, 1, 2, 3);
        var permissions2 = new AppPermissions(123, 1, 2, 3);

        // Act
        var hashCode1 = permissions1.GetHashCode();
        var hashCode2 = permissions2.GetHashCode();

        // Assert
        Assert.That(hashCode1, Is.EqualTo(hashCode2));
    }

    [Test]
    public void GetHashCode_ShouldReturnSameHashCode_ForEqualBadlyParsedAppPermissions() {
        // Arrange
        var permissions1 = AppPermissions.Parse("invalid", CultureInfo.InvariantCulture);
        var permissions2 = AppPermissions.Parse("invalid", CultureInfo.InvariantCulture);

        // Act
        var hashCode1 = permissions1.GetHashCode();
        var hashCode2 = permissions2.GetHashCode();

        // Assert
        Assert.That(hashCode1, Is.EqualTo(hashCode2));
    }

    [Test]
    public void ToString_ShouldReturnTextualRepresentation()
    {
        // Arrange
        var permissions = new AppPermissions(123, 1, 2, 3);

        // Act
        var result = permissions.ToString();

        // Assert
        Assert.That(result, Is.EqualTo(permissions.TextualRepresentation));
    }

    [Test]
    public void Operators_ShouldWorkCorrectly()
    {
        // Arrange
        var permissions1 = new AppPermissions(123, 1, 2, 3);
        var permissions2 = new AppPermissions(123, 1, 2, 3);
        var permissions3 = new AppPermissions(456, 4, 5, 6);

        // Act & Assert
        Assert.Multiple(() =>
        {
            Assert.That(permissions1 == permissions2, Is.True);
            Assert.That(permissions1 != permissions3, Is.True);
            Assert.That(permissions1 < permissions3, Is.True);
            Assert.That(permissions1 <= permissions2, Is.True);
            Assert.That(permissions3 > permissions1, Is.True);
            Assert.That(permissions3 >= permissions2, Is.True);
        });
    }
}

using Colonisation.Common;

namespace Colonisation.Points;

// See existing rules for ideas on points balance.

abstract class Rule
{
    // The return type is the point count followed by a short description.
    // Non-empty descriptions are concatenated to provide explanation.
    public abstract (double points, string description) Evaluate(ColonisationTarget starSystem, SystemBodies bodies);
}

// One point per close body and half a point per distant body. You can place at
// least one station around each.
class Bodies : Rule
{
    // Bodies further than this many light seconds from arrival
    // are considered harder to reach.
    public const int DistantThreshold = 50_000;

    public override (double points, string description) Evaluate(ColonisationTarget starSystem, SystemBodies bodies)
    {
        double points = bodies.bodies.Sum(body =>
        {
            return (body.distanceToArrival > DistantThreshold) ? 0.5 : 1;
        });
        return (points, $"{bodies.bodies.Count} bodies");
    }
}

// Additional points for landable worlds, proportionate to its gravity and doubles
// if it has an atmosphere. A body's gravity is a rough indicator of the number of
// outputs you can build on it.
class LandableBodies : Rule
{
    public override (double points, string description) Evaluate(ColonisationTarget starSystem, SystemBodies bodies)
    {
        IEnumerable<Body> landableWorlds = bodies.bodies.Where(body => body.isLandable);
        double points = landableWorlds.Sum(body =>
        {
            return !string.IsNullOrWhiteSpace(body.atmosphereType) ? body.gravity * 4 : body.gravity * 2;
        });
        return landableWorlds.Any()
            ? (points, $"{landableWorlds.Count()} landable worlds")
            : (0, "");
    }
}

//class MultipleStars : Rule
//{
//    public override (float points, string description) Evaluate(ColonisationTarget starSystem, SystemBodies bodies)
//    {
//        IEnumerable<Body> stars = bodies.bodies.Where(body => body.type == "Star");
//        bool isDistant = bodies.bodies.Any(body => body.type == "Star" && body.distanceToArrival > 50_000);
//        if(stars.Any() && isDistant)
//        {
//            return (1, $"Contains multiple but some distant stars: {string.Join(", ", stars.Select(bodyInfo => bodyInfo.name))}");
//        }
//        else if(stars.Any())
//        {
//            return (2, $"Contains multiple stars: {string.Join(", ", stars.Select(bodyInfo => bodyInfo.name))}");
//        }
//        else
//        {
//            return (0, "");
//        }
//    }
//}

// One point per ring.
class Rings : Rule
{
    public override (double points, string description) Evaluate(ColonisationTarget starSystem, SystemBodies bodies)
    {
        IEnumerable<Ring> rings = bodies.bodies.SelectMany(body => body.rings);
        return rings.Any()
            ? (rings.Count(), $"{rings.Count()} rings")
            : (0, "");
    }
}

// One point per asteroid belt.
class Belts : Rule
{
    public override (double points, string description) Evaluate(ColonisationTarget starSystem, SystemBodies bodies)
    {
        return bodies.belts.Count != 0
            ? (bodies.belts.Count, $"{bodies.belts.Count} asteroid belts")
            : (0, "");
    }
}

// Twenty points for terraformable. 
class TerraformableWorlds : Rule
{
    public override (double points, string description) Evaluate(ColonisationTarget starSystem, SystemBodies bodies)
    {
        IEnumerable<Body> terraformableWorlds = bodies.bodies.Where(body => !string.IsNullOrWhiteSpace(body.terraformingState) && body.terraformingState != "Not terraformable");
        return terraformableWorlds.Any()
            ? (20, $"{terraformableWorlds.Count()} terraformable worlds")
            : (0, "");
    }
}
**Overview**

Produce a list of potential systems for the upcoming colonization feature for the minor faction *EDA Kunti League* in the game **Elite Dangerous**.

**Use**

1. Download and extract `systemsWithCoordinates.json` from https://www.edsm.net/dump/systemsWithCoordinates.json.gz into `Colonisation.StarSystems`. Note that this is a large file (12+ GB) at the time of writing.
1. Download and extract `systemsPopulated.json` from https://www.edsm.net/en/nightly-dumps into `Colonisation.StarSystems`.
1. (Optional) Change the `minorFactionName` setting in `Colonisation.StarSystems\applicationSettings.config` to the name of your minor faction. It must match **exactly**.
1. Compile and run `Colonisation.StarSystems`. It may take a few minutes to run. Output is written to `colonisationTargets.csv` by default.
1. Copy `colonisationTargets.csv` into `Colonisation.Bodies`.
1. Compile and run `Colonisation.Bodies` to download relevant information about the bodies in these systems.
1. (Future) Copy `colonisationBodies.json` into `Colonisation.Points`.

**References**

1. EDSM System APIs: https://www.edsm.net/en/api-system-v1
1. ED Astro dumps: https://edastro.com/mapcharts/files.html (unused but potentially useful)
1. Inara API: https://inara.cz/elite/inara-api-devguide/ (unused but potentially useful)

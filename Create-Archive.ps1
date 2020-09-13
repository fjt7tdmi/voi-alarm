Param([switch] $Debug)

$version = "0.2.0"

$dir = "VoiAlarm/bin/Release"
if ($Debug)
{
    $dir = "VoiAlarm/bin/Debug"
}

$compressArgs = @{
    Path = "README.md", "LICENSE", "$dir/*.exe", "$dir/*.exe.config", "$dir/Settings.default.xml", "$dir/MaterialDesignColors.dll", "$dir/MaterialDesignThemes.Wpf.dll", "$dir/NAudio.dll"
    DestinationPath = "voi-alarm-$version.zip"
}

Compress-Archive -Force @compressArgs

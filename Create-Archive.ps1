$version = "0.1"
$dir = "VoiAlarm/bin/Release"

$compressArgs = @{
    Path = "README.md", "LICENSE", "$dir/*.exe", "$dir/*.exe.config", "$dir/Settings.default.xml", "$dir/MaterialDesignColors.dll", "$dir/MaterialDesignThemes.Wpf.dll", "$dir/NAudio.dll"
    DestinationPath = "voi-alarm-$version.zip"
}

Compress-Archive -Force @compressArgs

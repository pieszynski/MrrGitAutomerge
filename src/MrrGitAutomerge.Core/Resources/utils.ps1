param (
    $option
)

$CODE = 0
$ROOT = ( git rev-parse --show-toplevel )

if ( !$? ) {
    write-host "WorkDir: ${pwd}" -foregroundcolor gray
    write-host "This is not GIT repository." -foregroundcolor red
    exit 10
}

function last10messages {
    push-location -path $ROOT   
    $MESSAGE = ( ( git log -10 --pretty=%s ) -split '\n' ) | ? { $_.trim() -ne '' }
    $CODE = $lastExitCode
    $MESSAGE
    pop-location
    exit $CODE
}

switch ( $option ) {
    "last10messages" { last10messages }
    default { 
        write-host "WorkDir: ${pwd}" -foregroundcolor gray
        write-host "Please provide an option." -foregroundcolor red
        exit 20
    }
}

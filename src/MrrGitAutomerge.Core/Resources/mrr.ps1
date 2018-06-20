param(
    $MASTER_BRANCH = 'master',
    [int]$NO_PUSH = 0
)

[Console]::OutputEncoding = [Text.UTF8Encoding]::UTF8

$BRANCH = ((git status -sb --ignore-submodules) -split '\n')[0] -replace '^## ([^.]*).*?$','$1'
$HAS_MODS = ((git status -sb --ignore-submodules) -split '\n')[1]
$MESSAGE = (git log -1 --pretty=%B)
$MERGE_FLAG_FILE = "$(git rev-parse --show-toplevel)/.git/MERGE_HEAD"
$MERGE_STATUS = $false
$USER_COMMIT = (git log -1 --pretty=%h)
$REMOTE_COMMIT = ""

write-host "Running automerge script." -foregroundcolor gray
write-host "Working dir: $(pwd)" -foregroundcolor gray
write-host "Source branch: $BRANCH" -foregroundcolor gray
write-host "Destination branch $MASTER_BRANCH" -foregroundcolor gray

# there are uncommted files
if ($HAS_MODS) {
    write-host "There are uncommited changes in repository. Please commit them." -foregroundcolor red
    write-host "Command suggestion:"
    write-host ""
    write-host "\> git add . && git commit -m""$MESSAGE""" -foregroundcolor yellow
    exit 10
}

# switch to destination branch
if ($MASTER_BRANCH -ne $BRANCH) {
    # user commited to branch different than destination branch
    git checkout $MASTER_BRANCH
    if (!$?){
        write-host "Branch '$MASTER_BRANCH' does not exit." -foregroundcolor red
        exit 30
    }
    git pull origin $MASTER_BRANCH
    $REMOTE_COMMIT = (git log -1 --pretty=%h)
    git merge --no-commit --no-ff $BRANCH
    $MERGE_STATUS = $?
} else {
    # user commited to destination branch directly
    git fetch
    $REMOTE_COMMIT = (git log -1 FETCH_HEAD --pretty=%h)
    git merge --no-commit --no-ff FETCH_HEAD
    $MERGE_STATUS = $?
}

echo "Merge status: $MERGE_STATUS"

if (Test-Path $MERGE_FLAG_FILE) {
    if ($MERGE_STATUS) {
        write-host "Merge successful - commiting merge."
        git commit -m"$MESSAGE (Merge: $BRANCH $USER_COMMIT into $MASTER_BRANCH $REMOTE_COMMIT)"
        git checkout $BRANCH
        if ($MASTER_BRANCH -ne $BRANCH) {
            write-host "Updating user branch from $MASTER_BRANCH into $BRANCH"
            git merge --ff-only $MASTER_BRANCH
        }
    } else {
        write-host "Aborting merge - there are unresolveable conficts. User has to do it manually." -foregroundcolor red
        write-host "Please run following command on branch '$MASTER_BRANCH'"
        write-host "and then run this script once more."
        write-host ""
        write-host "\> git checkout $MASTER_BRANCH && git merge --no-commit --no-ff FETCH_HEAD"
        write-host ""
        write-host "==::DIFF START::========================"
        git diff
        write-host "==::END DIFF::=========================="
        git merge --abort
        git checkout $BRANCH
        exit 20
    }
}

if ($NO_PUSH -eq 0)
{
    git checkout $BRANCH
    git push origin $MASTER_BRANCH
}
else 
{
    write-host "[WARN] git-push not executed as requested."
}
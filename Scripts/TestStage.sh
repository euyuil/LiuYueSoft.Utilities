#!/bin/bash

# References:
# - https://gunnarpeipman.com/aspnet-core-code-coverage/
# - https://medium.com/@tonerdo/setting-up-coveralls-with-coverlet-for-a-net-core-project-2c8ec6c5dc58
# - https://github.com/csMACnz/Coveralls.net-Samples/blob/xunit-monocov-travisci/.travis.yml
# - https://github.com/tonerdo/coverlet
# - https://github.com/tonerdo/coverlet/issues/357#issuecomment-490404820

BASE_DIR=`pwd`
BUILD_REPORTS_DIR=$BASE_DIR/BuildReports
COVERALLS=~/.dotnet/tools/csmacnz.Coveralls

dotnet test --logger "trx;LogFileName=TestResults.trx" \
            --logger "xunit;LogFileName=TestResults.xml" \
            --results-directory $BUILD_REPORTS_DIR/UnitTests/ \
            /p:CollectCoverage=true \
            /p:CoverletOutput=$BUILD_REPORTS_DIR/Coverage.xml \
            /p:CoverletOutputFormat=opencover \
            /p:MergeWith=$BUILD_REPORTS_DIR/Coverage.xml \
            /p:Exclude="[xunit.*]*" \
            /maxcpucount:1

find $BASE_DIR

REPO_COMMIT_AUTHOR=$(git show -s --pretty=format:"%cn")
REPO_COMMIT_AUTHOR_EMAIL=$(git show -s --pretty=format:"%ce")
REPO_COMMIT_MESSAGE=$(git show -s --pretty=format:"%s")

echo "TRAVIS_COMMIT=$TRAVIS_COMMIT"
echo "TRAVIS_BRANCH=$TRAVIS_BRANCH"
echo "REPO_COMMIT_AUTHOR=$REPO_COMMIT_AUTHOR"
echo "REPO_COMMIT_AUTHOR_EMAIL=$REPO_COMMIT_AUTHOR_EMAIL"
echo "REPO_COMMIT_MESSAGE=$REPO_COMMIT_MESSAGE"
echo "TRAVIS_JOB_ID=$TRAVIS_JOB_ID"

$COVERALLS --opencover -i $BUILD_REPORTS_DIR/Coverage.xml \
           --commitId $TRAVIS_COMMIT \
           --commitBranch $TRAVIS_BRANCH \
           --commitAuthor "$REPO_COMMIT_AUTHOR" \
           --commitEmail "$REPO_COMMIT_AUTHOR_EMAIL" \
           --commitMessage "$REPO_COMMIT_MESSAGE" \
           --jobId $TRAVIS_JOB_ID \
           --serviceName "travis-ci" \
           --useRelativePaths

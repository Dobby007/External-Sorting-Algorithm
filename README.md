# External Sorting Algorithm
External sorting algorithm written in C# that can be used for sorting of big files that don't fit into RAM. This specific .NET application sorts files with the following format:

    415. Apple
    30432. Something something something
    1. Apple
    32. Cherry is the best
    2. Banana is yellow

Each record contains number and corresponding string separated by dot and space.
First the application compares the string part of records and in the case of their equality it compares numbers related to each string. So the output  file in the example above will be the following:

    1. Apple
    415. Apple
    2. Banana is yellow
    32. Cherry is the best
    30432. Something something something

This algorithm uses two-way merging, described here:
http://faculty.simpson.edu/lydia.sinapova/www/cmsc250/LN250_Weiss/L17-ExternalSort.htm#analysis

Below is a quote from the URL above (also it can be found in the `book` folder of this repository):


    The algorithm requires [log(N/M)] passes plus the initial run-constructing pass.

    Each pass merges runs of length r to obtain runs of length 2*r.
    The first runs are of length M. The last run would be of length N.

    Let's assume that N is a multiple of M.

    Initial situation:

    1st tape contains N records = M records * N/M runs

    After storing the runs on two tapes, each contains half of the runs:

    2 tapes * M records_per_run * (1/2)(N/M) runs = N records

    After merge 1st pass - double the length of the runs, halve the number of the runs:

    2 tapes * 2M records_per_run * (1/2)(1/2)(N/M) runs = N records

    After merge 2nd pass :

    2 tapes * 4M records_per_run * (1/4)(1/2) (N/M) runs = N records

    ......

    After merge s-th pass:

    2 tapes * 2s M records_per_run * (1/2s)(1/2)(N/M) runs = N records

    ......

    Thus the length of the runs after the s-th merge is 2sM.

    After the last merge there is only one run equal to the whole file:

    2sM = N

    2s = N/M

    s = log(N/M)

    if s is the last merge, s = log(N/M)

    At each pass we process N records, so the complexity is O(Nlog(N/M))


License
====

MIT

# External Sorting Algorithm
External sorting algorithm written in C# that can be used for sorting of big files that don't fit into RAM. This specific .NET application sorts files with the following format:

    415. Apple
    30432. Something something something
    1. Apple
    32. Cherry is the best
    2. Banana is yellow

Each record contains number and corresponding string separated by dot and space.
At first the application compares the string part of different records and in the case of their equality it compares numbers related to each string. So the output  file in the example above will be the following:

    1. Apple
    415. Apple
    2. Banana is yellow
    32. Cherry is the best
    30432. Something something something

This algorithm uses two-way merging, described here:
http://faculty.simpson.edu/lydia.sinapova/www/cmsc250/LN250_Weiss/L17-ExternalSort.htm#analysis

Below is a quote from the URL above (also it can be found in the `book` folder of this repository):

    Algorithm flow
    -----
    Assumptions: 
        four tapes: 

        two for input - Ta1, Ta2, 
        two for output - Tb1, Tb2.
        Initially the file is on Ta1.
        N records on Ta1
        M records can fit in the memory

    Step 1: Break the file into blocks of size M, [N/M]+1 blocks

    Step 2: Sorting the blocks:

        read a block, sort, store on Tb1
        read a block, sort, store on Tb2,
        read a block, sort, store on Tb1,
        etc, alternatively writing on Tb1 and Tb2
    Each sorted block is called a run.
    Each output tape will contain half of the runs

    Step 3: Merge:

    a. From Tb1, Tb2 to Ta1, Ta2. 
    Merge the first run on Tb1 and the first run on Tb2, and store the result on Ta1:
        Read two records in main memory, compare, store the smaller on Ta1

        Read the next record (from Tb1 or Tb2 - the tape that contained the record 
        stored on Ta1) compare, store on Ta1, etc.

    Merge the second run on Tb1 and the second run on Tb2, store the result on Ta2.
    Merge the third run on Tb1 and the third run on Tb2, store the result on Ta1.
    Etc, storing the result alternatively on Ta1 and Ta2.
    Now Ta1 and Ta2 will contain sorted runs twice the size of the previous runs on Tb1 and Tb2

    b. From Ta1, Ta2 to Tb1, Tb2. 
    Merge the first run on Ta1 and the first run on Ta2, and store the result on Tb1.
    Merge the second run on Ta1 and the second run on Ta2, store the result on Tb2
    Etc, merge and store alternatively on Ta1 and Ta2.
    c. Repeat the process until only one run is obtained. This would be the sorted file

    Analysis of two-way merge
    ----
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

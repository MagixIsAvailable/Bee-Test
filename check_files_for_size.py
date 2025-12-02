import os

# GitHub's limit is 100MB. We warn at 99MB to be safe.
SIZE_LIMIT_MB = 99
BYTES_LIMIT = SIZE_LIMIT_MB * 1024 * 1024

# Folders to skip (saves time and avoids false alarms)
IGNORE_FOLDERS = {'.git', 'Library', 'Temp', 'Obj', 'Build', 'Builds', '.vs'}

def check_files():
    print(f" Scanning for files larger than {SIZE_LIMIT_MB}MB...")
    found_big_files = False
    
    # Get the current folder where the script is running
    start_dir = os.getcwd()

    for root, dirs, files in os.walk(start_dir):
        # Modify 'dirs' in-place to skip ignored folders
        dirs[:] = [d for d in dirs if d not in IGNORE_FOLDERS]

        for name in files:
            filepath = os.path.join(root, name)
            try:
                size = os.path.getsize(filepath)
                if size > BYTES_LIMIT:
                    size_mb = size / (1024 * 1024)
                    print(f" BIG FILE FOUND: {size_mb:.2f} MB")
                    print(f"   -> Path: {filepath}")
                    found_big_files = True
            except OSError:
                continue

    if not found_big_files:
        print(" Safe! No files larger than 100MB found.")
    else:
        print("  WARNING: You cannot upload the files listed above to GitHub.")

if __name__ == "__main__":
    check_files()
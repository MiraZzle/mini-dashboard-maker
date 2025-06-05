import os

def get_cs_files_size(start_path):
    total_size = 0
    for dirpath, dirnames, filenames in os.walk(start_path):
        for filename in filenames:
            if filename.endswith(".cs"):
                file_path = os.path.join(dirpath, filename)
                total_size += os.path.getsize(file_path)
    return total_size

if __name__ == "__main__":
    folder_path = input("Enter the path to the folder: ").strip()
    
    if not os.path.isdir(folder_path):
        print("Invalid folder path.")
    else:
        size_in_bytes = get_cs_files_size(folder_path)
        size_in_kb = size_in_bytes / 1024
        size_in_mb = size_in_bytes / (1024 * 1024)

        print(f"Total size of .cs files:")
        print(f"Bytes: {size_in_bytes}")
        print(f"KB: {size_in_kb:.2f}")
        print(f"MB: {size_in_mb:.2f}")

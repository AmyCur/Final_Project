import os
from PIL import Image

def pixelate_image(image_path, pixel_size, output_path):

    # Open the image
    img = Image.open(image_path)
    
    # Resize image to pixelate by reducing the size and resizing back
    img_resized = img.resize(pixel_size, resample=Image.NEAREST)
    img_pixelated = img_resized.resize(img.size, Image.NEAREST)
    
    # Save the pixelated image
    img_pixelated.save(output_path)

def pixelate_images_in_folder(folder_path, resolution):
    pixelated_folder = os.path.join(folder_path, 'pixelated')
    if not os.path.exists(pixelated_folder):
        os.makedirs(pixelated_folder)
    
    # Loop through the files in the given folder
    for filename in os.listdir(folder_path):
        if filename.lower().endswith(('.png', '.jpg', '.jpeg')):  
            image_path = os.path.join(folder_path, filename)
            output_path = os.path.join(pixelated_folder, f"{filename}")
            print(f"Processing: {image_path}")
            pixelate_image(image_path, resolution ,output_path)
            print(f"Saved pixelated image to: {output_path}")

if __name__ == "__main__":
    folder_path = os.getcwd()  # Prompt user for folder path
    try:
        resolution = int(input("What is the desired resolution (Default = 256) >>> "))
    except:
        resolution=256
    pixelate_images_in_folder(folder_path, (resolution, resolution))


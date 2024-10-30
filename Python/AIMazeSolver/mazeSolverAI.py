import math
import pygame
import os
import neat
pygame.font.init()


GEN = 0
STAT_FONT = pygame.font.SysFont("comicsans", 10)
pygame.display.set_caption("Maze Solver")

map_count = 0
for file in os.listdir("imgs"):
    if file.startswith("Maze"):
            map_count+=1

#load all images
maze_images = [pygame.image.load(os.path.join("imgs","Maze" + str(x) + ".png")) for x in range(1,map_count+1)]
cube_image = pygame.transform.scale(pygame.image.load(os.path.join("imgs", "Cube.png")), (10,10))
circle_image = pygame.transform.scale(pygame.image.load(os.path.join("imgs", "Circle.png")), (20,20))


class Cube:
    VEL = 10
    IMG = cube_image

    def __init__(self,x,y,angle):
        self.x = x
        self.y = y
        self.height = self.IMG.get_height()
        self.width = self.IMG.get_width()
        self.center = [self.x + (self.width/2), self.y + (self.height/2)]
        self.rotate_surface = self.IMG
        self.angle = angle
        self.is_alive = True
        self.radars = []
        self.radars_for_draw = []

    def move(self):
        angle_radians = math.radians(360-self.angle)

        # Calculate the change in x and y based on the angle
        dx = self.VEL * math.cos(angle_radians)
        dy = self.VEL * math.sin(angle_radians)

        self.x += dx
        self.y += dy


    def draw(self,win):
        win.blit(self.rotate_surface,(self.x, self.y))
        self.draw_radar(win)

    def draw_radar(self, win):
        for r in self.radars:
            pos, dist = r
            pygame.draw.line(win, (0, 255, 0), self.center, pos, 1)
            pygame.draw.circle(win, (0, 255, 0), pos, 5)

    def collide(self,map):
        for p in self.collision_points:
            col_x,col_y = int(p[0]),int(p[1])

            #running out of map also kills cubes
            if col_y >= map.get_height() or col_y <= 0 or col_x <= 0 or col_x >= map.get_width():
                self.is_alive = False
                break

            if map.get_at((col_x,col_y)) == (0, 0, 0):
                #car ran into wall and is now dead
                self.is_alive = False
                break


    def calculate_collision_points(self):
        center = (self.x + (self.width/2), self.y + (self.height/2))
        top_left = (center[0] - (self.width/2), center[1] - (self.height/2))
        top_right = (center[0] + (self.width/2), center[1] - (self.height/2))
        bottom_left = (center[0] - (self.width/2), center[1] + (self.height/2))
        bottom_right = (center[0] + (self.width/2), center[1] + (self.height/2))

        self.collision_points = [center, top_left, top_right, bottom_left, bottom_right]
        self.center = center

    def rot_center(self,image,angle):
        rotated_image = pygame.transform.rotate(image, angle)
        return rotated_image

    def check_radar(self, degree, map):
        #calculate radars
        angle_radians = math.radians(360-(self.angle + degree))
        len = 0
        x = int(self.center[0])
        y = int(self.center[1])

        if x >= map.get_width():
            x  = map.get_width() -1
        if x <= 0:
            x = 1
        if y >= map.get_height():
            y = map.get_height() -1
        if y <= 0:
            y = 1

        while not map.get_at((x, y)) == (0, 0, 0) and len < 300:
            len = len + 1
            x = int(self.center[0] + math.cos(angle_radians) * len)
            y = int(self.center[1] + math.sin(angle_radians) * len)

            if y >= map.get_height() or y <= 0 or x <= 0 or x >= map.get_width():
                break

        dist = int(math.sqrt(math.pow(x - self.center[0], 2) + math.pow(y - self.center[1], 2)))
        self.radars.append([(x, y), dist])

    def get_data(self):
        #radars: distance to any wall -> input of activation function
        radars = self.radars
        ret = [0,0,0,0,0,0]
        for i, r in enumerate(radars):
            ret[i] = int(r[1] / 30)

        return ret

class Circle:
    IMG = circle_image

    def __init__(self,x,y):
        self.x = x
        self.y = y
        self.height = self.IMG.get_height()
        self.width = self.IMG.get_width()

    def draw(self,win):
        win.blit(self.IMG, (self.x, self.y))

    #did any cube collide with this circle
    def collide(self,cube):
        circle_mask = pygame.mask.from_surface(self.IMG.convert_alpha())
        cube_mask = pygame.mask.from_surface(cube.IMG.convert_alpha())
        if cube_mask.overlap(circle_mask,(self.x-round(cube.x),self.y-round(cube.y))):
            return True

        return False

class Maze:

    def __init__(self,x,y,img_index):
        self.x = x
        self.y = y
        self.IMG = maze_images[img_index]
        self.img_index = img_index
        self.height = self.IMG.get_height()
        self.width = self.IMG.get_width()

    def draw(self,win):
        win.blit(self.IMG, (self.x,self.y))


def draw_window(win,timer,cubes,circle,maze,pop,gen):
    maze.draw(win)
    circle.draw(win)
    for cube in cubes:
        cube.draw(win)

    #render stats on top left corner on screen
    black = (0,0,0)
    white = (255,255,255)
    curr_color = white

    text_timer = STAT_FONT.render("Timer: " + str(int(timer)), 1, curr_color)
    win.blit(text_timer, (30, 10))

    text_gen = STAT_FONT.render("Generation: " + str(gen),1,curr_color)
    win.blit(text_gen, (30, 10 + text_timer.get_height()))

    text_population = STAT_FONT.render("Population: " + str(pop), 1, curr_color)
    win.blit(text_population, (30, 10 + text_gen.get_height() + text_timer.get_height()))

    pygame.display.update()

#this is run for every generation
def main(genomes, config):
    global GEN
    GEN += 1
    nets = []
    cubes = []
    ge = []

    maze = Maze(0, 0, 1)
    WIN_WIDTH, WIN_HEIGHT = maze.IMG.get_width(), maze.IMG.get_height()
    win = pygame.display.set_mode((WIN_WIDTH, WIN_HEIGHT))

    circle = Circle(maze.IMG.get_width()/2 - 25, 0)
    cube_start_angle = [0,180] # start angle and pos of cube depends on maze
    cube_start_pos = [(WIN_WIDTH/2,WIN_HEIGHT - cube_image.get_height(),cube_start_angle[maze.img_index]),
                      (WIN_WIDTH/2,WIN_HEIGHT - cube_image.get_height(),cube_start_angle[maze.img_index])] #per maze


    for _,g in genomes:
        g.fitness = 0
        net = neat.nn.FeedForwardNetwork.create(g,config)
        nets.append(net)
        cubes.append(Cube(cube_start_pos[maze.img_index][0],cube_start_pos[maze.img_index][1] ,cube_start_angle[maze.img_index]))
        ge.append(g)


    timer = 0
    clock = pygame.time.Clock()
    start_ticks = pygame.time.get_ticks()

    run = True
    while run and len(cubes) > 0:
        clock.tick(30)
        timer = (pygame.time.get_ticks()-start_ticks)/1000 #timer in seconds

        for event in pygame.event.get():
            if event.type == pygame.QUIT:
                run = False
                pygame.quit()
                quit()
                break

        if len(cubes) > 0:
            for x,cube in enumerate(cubes):
                ge[x].fitness += 0.1 #increase fitness for those that survived longer

                output = nets[cubes.index(cube)].activate((cube.get_data()))
                i = output.index(max(output))
                if i == 0:
                    cube.angle += 15
                else:
                    cube.angle -= 15

                #rotate cube
                cube.rotate_surface = cube.rot_center(cube.IMG,cube.angle)

                cube.move()
                cube.calculate_collision_points() #points on the edge of cube
                cube.collide(maze.IMG)  # after running this function we can now access is_alive component

                if circle.collide(cube):
                    #check if any cube reached the circle
                    ge[cubes.index(cube)].fitness += 10
                    nets.pop(cubes.index(cube))
                    ge.pop(cubes.index(cube))
                    cubes.pop(cubes.index(cube))


                cube.radars.clear()
                radar_angles = [-180,-90,-45,0,45,90]
                for d in radar_angles:
                    cube.check_radar(d, maze.IMG)


                if cube.is_alive == False:
                    # cube ran into wall
                    ge[cubes.index(cube)].fitness -= 1
                    nets.pop(cubes.index(cube))
                    ge.pop(cubes.index(cube))
                    cubes.pop(cubes.index(cube))

        draw_window(win,timer,cubes,circle,maze,len(cubes),GEN)



if __name__ == '__main__':
    local_dir = os.path.dirname(__file__)
    config_path = os.path.join(local_dir, 'config-feedforward.txt')

    config = neat.config.Config(neat.DefaultGenome, neat.DefaultReproduction,
                                neat.DefaultSpeciesSet, neat.DefaultStagnation,
                                config_path)

    # Create the population, which is the top-level object for a NEAT run.
    p = neat.Population(config)

    # Add a stdout reporter to show progress in the terminal.
    p.add_reporter(neat.StdOutReporter(True))
    p.add_reporter(neat.StatisticsReporter())

    # p.add_reporter(neat.Checkpointer(5))

    # Run for up to n generations.
    winner = p.run(main, 100)

    # show final stats
    print('\nBest genome:\n{!s}'.format(winner))



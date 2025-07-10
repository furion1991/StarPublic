import Image from 'next/image'

import classes from './PointsLeaderboard.module.scss'

export const PointsLeaderboard = () => {
  const leaders = [
    {
      place: 1,
      nickName: 'rendel.spot 1',
      prize: 20_980,
      points: 20_980
    },
    {
      place: 2,
      nickName: 'rendel.spot 2',
      prize: 20_980,
      points: 20_980
    },
    {
      place: 3,
      nickName: 'rendel.spot 3',
      prize: 20_980,
      points: 20_980
    },
    {
      place: 4,
      nickName: 'rendel.spot 4',
      prize: 20_980,
      points: 20_980
    },
    {
      place: 5,
      nickName: 'rendel.spot 5',
      prize: 20_980,
      points: 20_980
    }
  ]

  return (
    <div className={classes.pointsLeaderboard}>
      <div className={classes.labels}>
        <span>Место</span>
        <span>Игрок</span>
        <span>Приз</span>
        <span>Баллов</span>
      </div>

      <div className={classes.rows}>
        {leaders.map(({ place, nickName, prize, points }) => {
          return (
            <div className={classes.row} key={place}>
              <div className={classes.place}>
                <div>{place}</div>
              </div>

              <div className={classes.player}>
                <Image src='/placeholders/quest-player.png' width={39} height={49} alt='Игрок' />

                <span>{nickName}</span>
              </div>

              <div className={classes.prize}>
                <Image src='/img/gem.png' width={30} height={33} alt='Гем' />

                <span>{new Intl.NumberFormat('de-DE').format(prize)}</span>
              </div>

              <div className={classes.points}>
                <Image src='/icons/coin.svg' width={24} height={26} alt='Монета' />

                <span>{new Intl.NumberFormat('de-DE').format(points)}</span>
              </div>
            </div>
          )
        })}
      </div>
    </div>
  )
}
